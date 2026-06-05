using Microsoft.AspNetCore.Mvc;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMovesLogistics.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;

        public ContractsController(IContractService contractService)
        {
            _contractService = contractService;
        }

        // GET /api/contracts?startDate=&endDate=&status=
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetContracts(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] ContractStatus? status)
        {
            var contracts = await _contractService.SearchContractsAsync(startDate, endDate, status);
            var response = contracts.Select(ContractResponseDto.FromEntity);
            return Ok(response);
        }

        // POST /api/contracts
        [HttpPost]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateContract([FromBody] CreateContractRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = request.ToEntity();
            await _contractService.CreateContractAsync(contract);

            var created = await _contractService.GetContractByIdAsync(contract.Id);
            if (created == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Created($"/api/contracts/{created.Id}", ContractResponseDto.FromEntity(created));
        }
    }
}
