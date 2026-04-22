using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechMoves_Logistics.Data;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Controllers
{
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestService _reqService;
        private readonly IContractService _contractService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(IServiceRequestService reqService, IContractService contractService, ICurrencyService currencyService)
        {
            _reqService = reqService;
            _contractService = contractService;
            _currencyService = currencyService;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index() => View(await _reqService.GetAllAsync());

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _reqService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create(int? contractId)
        {
            ViewData["ContractId"] = new SelectList(await _contractService.GetAllContractsAsync(), "Id", "ServiceLevel", contractId);
            ViewBag.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
            return View();
        }

        // POST: ServiceRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContractId,Description,CostUSD,Status")] ServiceRequest serviceRequest)
        {
            try
            {
                // Currency conversion before saving
                var rate = await _currencyService.GetUsdToZarRateAsync();
                serviceRequest.ExchangeRateUsed = rate;

                if (serviceRequest.CostUSD.HasValue)
                    serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD.Value, rate);

                // Business rule validation happens inside this service call
                await _reqService.CreateServiceRequestAsync(serviceRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Catches "Expired" or "OnHold" contract violations from Service Layer
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            ViewData["ContractId"] = new SelectList(await _contractService.GetAllContractsAsync(), "Id", "ServiceLevel", serviceRequest.ContractId);
            ViewBag.ExchangeRate = await _currencyService.GetUsdToZarRateAsync();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _reqService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            ViewData["ContractId"] = new SelectList(await _contractService.GetAllContractsAsync(), "Id", "ServiceLevel", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // POST: ServiceRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ContractId,Description,CostZAR,CostUSD,ExchangeRateUsed,Status,CreatedAt")] ServiceRequest serviceRequest)
        {
            if (id != serviceRequest.Id) return NotFound();

            if (ModelState.IsValid)
            {
                await _reqService.UpdateAsync(serviceRequest);
                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(await _contractService.GetAllContractsAsync(), "Id", "ServiceLevel", serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _reqService.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _reqService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
