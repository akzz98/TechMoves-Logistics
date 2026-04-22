using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMoves_Logistics.Data;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Repositories.Interfaces;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Controllers
{
    public class ContractsController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IClientRepository _clientRepo;
        private readonly IFileService _fileService;

        public ContractsController(IContractService contractService, IClientRepository clientRepo, IFileService fileService)
        {
            _contractService = contractService;
            _clientRepo = clientRepo;
            _fileService = fileService;
        }

        // GET: Contracts with search filter
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _contractService.SearchContractsAsync(startDate, endDate, status);
            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name");
            return View();
        }

        // POST: Contracts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel")] Contract contract, IFormFile? signedAgreement)
        {
            if (ModelState.IsValid)
            {
                // Handle PDF upload via FileService
                if (signedAgreement != null)
                {
                    try
                    {
                        contract.SignedAgreementPath = await _fileService.SavePdfAsync(signedAgreement);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name");
                        return View(contract);
                    }
                }

                await _contractService.CreateContractAsync(contract);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name");
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // POST: Contracts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,StartDate,EndDate,Status,ServiceLevel,SignedAgreementPath")] Contract contract, IFormFile? signedAgreement)
        {
            if (id != contract.Id) return NotFound();

            if (ModelState.IsValid)
            {
                // Allow replacing the PDF on edit
                if (signedAgreement != null)
                {
                    try
                    {
                        // Delete old file if it exists
                        if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                            _fileService.DeleteFile(contract.SignedAgreementPath);

                        contract.SignedAgreementPath = await _fileService.SavePdfAsync(signedAgreement);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name", contract.ClientId);
                        return View(contract);
                    }
                }

                await _contractService.UpdateContractAsync(contract);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(await _clientRepo.GetAllAsync(), "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractService.GetContractByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Also delete the PDF file from server when contract is deleted
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract != null && !string.IsNullOrEmpty(contract.SignedAgreementPath))
                _fileService.DeleteFile(contract.SignedAgreementPath);

            await _contractService.DeleteContractAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // GET: Download PDF Agreement
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", contract.SignedAgreementPath.TrimStart('/'));
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Contract_Agreement_{id}.pdf");
        }
    }
}
