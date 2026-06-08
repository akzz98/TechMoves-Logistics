using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Services;

namespace TechMoves_Logistics.Controllers
{
    [Authorize]
    public class ClientsController : Controller
    {
        private readonly IClientsApiClient _clientsApiClient;

        public ClientsController(IClientsApiClient clientsApiClient)
        {
            _clientsApiClient = clientsApiClient;
        }

        // GET: Clients
        public async Task<IActionResult> Index()
        {
            return View(await _clientsApiClient.GetAllAsync());
        }

        // GET: Clients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientsApiClient.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create() => View();

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (ModelState.IsValid)
            {
                await _clientsApiClient.CreateAsync(client);
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientsApiClient.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ContactDetails,Region")] Client client)
        {
            if (id != client.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var updated = await _clientsApiClient.UpdateAsync(id, client);
                if (updated == null) return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var client = await _clientsApiClient.GetByIdAsync(id.Value);
            if (client == null) return NotFound();

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deleted = await _clientsApiClient.DeleteAsync(id);
            if (!deleted) return NotFound();

            return RedirectToAction(nameof(Index));
        }
    }
}
