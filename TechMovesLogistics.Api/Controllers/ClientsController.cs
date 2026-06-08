using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Repositories.Interfaces;

namespace TechMovesLogistics.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        public ClientsController(IClientRepository clientRepository)
        {
            _clientRepository = clientRepository;
        }

        // GET /api/clients
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClients()
        {
            var clients = await _clientRepository.GetAllAsync();
            return Ok(clients.Select(ClientResponseDto.FromEntity));
        }

        // GET /api/clients/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            return Ok(ClientResponseDto.FromEntity(client));
        }

        // POST /api/clients
        [HttpPost]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateClient([FromBody] CreateClientRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = request.ToEntity();
            await _clientRepository.AddAsync(client);

            return Created($"/api/clients/{client.Id}", ClientResponseDto.FromEntity(client));
        }

        // PUT /api/clients/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ClientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateClient(int id, [FromBody] UpdateClientRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            client.Name = request.Name;
            client.ContactDetails = request.ContactDetails;
            client.Region = request.Region;

            await _clientRepository.UpdateAsync(client);
            return Ok(ClientResponseDto.FromEntity(client));
        }

        // DELETE /api/clients/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _clientRepository.GetByIdAsync(id);
            if (client == null)
                return NotFound();

            await _clientRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
