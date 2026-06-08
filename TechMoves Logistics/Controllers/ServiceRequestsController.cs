using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Services;

namespace TechMoves_Logistics.Controllers
{
    [Authorize]
    public class ServiceRequestsController : Controller
    {
        private readonly IServiceRequestsApiClient _serviceRequestsApiClient;
        private readonly IContractsApiClient _contractsApiClient;
        private readonly ICurrencyApiClient _currencyApiClient;

        public ServiceRequestsController(
            IServiceRequestsApiClient serviceRequestsApiClient,
            IContractsApiClient contractsApiClient,
            ICurrencyApiClient currencyApiClient)
        {
            _serviceRequestsApiClient = serviceRequestsApiClient;
            _contractsApiClient = contractsApiClient;
            _currencyApiClient = currencyApiClient;
        }

        // GET: ServiceRequests
        public async Task<IActionResult> Index() => View(await _serviceRequestsApiClient.GetAllAsync());

        // GET: ServiceRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestsApiClient.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // GET: ServiceRequests/Create
        public async Task<IActionResult> Create(int? contractId)
        {
            ViewData["ContractId"] = new SelectList(
                await _contractsApiClient.SearchAsync(null, null, null),
                "Id",
                "ServiceLevel",
                contractId);
            ViewBag.ExchangeRate = await _currencyApiClient.GetUsdToZarRateAsync();
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
                // Currency conversion and business rules handled by the API
                await _serviceRequestsApiClient.CreateAsync(serviceRequest);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                // Catches "Expired" or "OnHold" contract violations from the API
                ModelState.AddModelError("", ex.Message);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
            }

            ViewData["ContractId"] = new SelectList(
                await _contractsApiClient.SearchAsync(null, null, null),
                "Id",
                "ServiceLevel",
                serviceRequest.ContractId);
            ViewBag.ExchangeRate = await _currencyApiClient.GetUsdToZarRateAsync();
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestsApiClient.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            ViewData["ContractId"] = new SelectList(
                await _contractsApiClient.SearchAsync(null, null, null),
                "Id",
                "ServiceLevel",
                serviceRequest.ContractId);
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
                var updated = await _serviceRequestsApiClient.UpdateAsync(id, serviceRequest);
                if (updated == null) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            ViewData["ContractId"] = new SelectList(
                await _contractsApiClient.SearchAsync(null, null, null),
                "Id",
                "ServiceLevel",
                serviceRequest.ContractId);
            return View(serviceRequest);
        }

        // GET: ServiceRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var serviceRequest = await _serviceRequestsApiClient.GetByIdAsync(id.Value);
            if (serviceRequest == null) return NotFound();

            return View(serviceRequest);
        }

        // POST: ServiceRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _serviceRequestsApiClient.DeleteAsync(id);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
