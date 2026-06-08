using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Services;

namespace TechMoves_Logistics.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly IContractsApiClient _contractsApiClient;
        private readonly IClientsApiClient _clientsApiClient;

        public ContractsController(IContractsApiClient contractsApiClient, IClientsApiClient clientsApiClient)
        {
            _contractsApiClient = contractsApiClient;
            _clientsApiClient = clientsApiClient;
        }

        // GET: Contracts with search filter
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, ContractStatus? status)
        {
            var contracts = await _contractsApiClient.SearchAsync(startDate, endDate, status);
            return View(contracts);
        }

        // GET: Contracts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractsApiClient.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // GET: Contracts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name");
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
                var created = await _contractsApiClient.CreateAsync(contract);

                if (signedAgreement != null)
                {
                    var uploaded = await _contractsApiClient.UploadAgreementAsync(created.Id, signedAgreement);
                    if (uploaded == null)
                    {
                        ModelState.AddModelError("", "Invalid PDF file. Only PDF files are allowed.");
                        ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name");
                        return View(contract);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name");
            return View(contract);
        }

        // GET: Contracts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractsApiClient.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name", contract.ClientId);
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
                var updated = await _contractsApiClient.UpdateAsync(id, contract);
                if (updated == null) return NotFound();

                if (signedAgreement != null)
                {
                    var uploaded = await _contractsApiClient.UploadAgreementAsync(id, signedAgreement);
                    if (uploaded == null)
                    {
                        ModelState.AddModelError("", "Invalid PDF file. Only PDF files are allowed.");
                        ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name", contract.ClientId);
                        return View(contract);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(await _clientsApiClient.GetAllAsync(), "Id", "Name", contract.ClientId);
            return View(contract);
        }

        // GET: Contracts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var contract = await _contractsApiClient.GetByIdAsync(id.Value);
            if (contract == null) return NotFound();

            return View(contract);
        }

        // POST: Contracts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _contractsApiClient.DeleteAsync(id);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // GET: Download PDF Agreement
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var fileBytes = await _contractsApiClient.DownloadAgreementAsync(id);
            if (fileBytes == null) return NotFound();

            return File(fileBytes, "application/pdf", $"Contract_Agreement_{id}.pdf");
        }
    }
}
