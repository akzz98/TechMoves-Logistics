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
        private readonly IFileService _fileService;

        public ContractsController(IContractService contractService, IFileService fileService)
        {
            _contractService = contractService;
            _fileService = fileService;
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

        // GET /api/contracts/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetContract(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
                return NotFound();

            return Ok(ContractResponseDto.FromEntity(contract));
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

        // PUT /api/contracts/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateContract(int id, [FromBody] UpdateContractRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
                return NotFound();

            request.ApplyTo(contract);
            await _contractService.UpdateContractAsync(contract);

            var updated = await _contractService.GetContractByIdAsync(id);
            if (updated == null)
                return NotFound();

            return Ok(ContractResponseDto.FromEntity(updated));
        }

        // PATCH /api/contracts/{id}/status
        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateContractStatus(int id, [FromBody] UpdateContractStatusRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _contractService.UpdateContractStatusAsync(id, request.Status);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            var updated = await _contractService.GetContractByIdAsync(id);
            if (updated == null)
                return NotFound();

            return Ok(ContractResponseDto.FromEntity(updated));
        }

        // POST /api/contracts/{id}/agreement
        [HttpPost("{id}/agreement")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ContractResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadAgreement(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("A PDF file is required.");

            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
                return NotFound();

            try
            {
                if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                    _fileService.DeleteFile(contract.SignedAgreementPath);

                contract.SignedAgreementPath = await _fileService.SavePdfAsync(file);
                await _contractService.UpdateContractAsync(contract);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            var updated = await _contractService.GetContractByIdAsync(id);
            if (updated == null)
                return NotFound();

            return Ok(ContractResponseDto.FromEntity(updated));
        }

        // GET /api/contracts/{id}/agreement
        [HttpGet("{id}/agreement")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadAgreement(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null || string.IsNullOrEmpty(contract.SignedAgreementPath))
                return NotFound();

            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                contract.SignedAgreementPath.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
            return File(fileBytes, "application/pdf", $"Contract_Agreement_{id}.pdf");
        }

        // DELETE /api/contracts/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteContract(int id)
        {
            var contract = await _contractService.GetContractByIdAsync(id);
            if (contract == null)
                return NotFound();

            if (!string.IsNullOrEmpty(contract.SignedAgreementPath))
                _fileService.DeleteFile(contract.SignedAgreementPath);

            await _contractService.DeleteContractAsync(id);
            return NoContent();
        }
    }
}
