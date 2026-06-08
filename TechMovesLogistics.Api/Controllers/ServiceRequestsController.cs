using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMovesLogistics.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/servicerequests")]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly IServiceRequestService _serviceRequestService;
        private readonly ICurrencyService _currencyService;

        public ServiceRequestsController(
            IServiceRequestService serviceRequestService,
            ICurrencyService currencyService)
        {
            _serviceRequestService = serviceRequestService;
            _currencyService = currencyService;
        }

        // GET /api/servicerequests
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServiceRequests()
        {
            var requests = await _serviceRequestService.GetAllAsync();
            return Ok(requests.Select(ServiceRequestResponseDto.FromEntity));
        }

        // GET /api/servicerequests/{id}
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetServiceRequest(int id)
        {
            var request = await _serviceRequestService.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            return Ok(ServiceRequestResponseDto.FromEntity(request));
        }

        // POST /api/servicerequests
        [HttpPost]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateServiceRequest([FromBody] CreateServiceRequestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var serviceRequest = request.ToEntity();

            var rate = await _currencyService.GetUsdToZarRateAsync();
            serviceRequest.ExchangeRateUsed = rate;

            if (serviceRequest.CostUSD.HasValue)
                serviceRequest.CostZAR = _currencyService.ConvertUsdToZar(serviceRequest.CostUSD.Value, rate);

            try
            {
                await _serviceRequestService.CreateServiceRequestAsync(serviceRequest);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            var created = await _serviceRequestService.GetByIdAsync(serviceRequest.Id);
            if (created == null)
                return StatusCode(StatusCodes.Status500InternalServerError);

            return Created($"/api/servicerequests/{created.Id}", ServiceRequestResponseDto.FromEntity(created));
        }

        // PUT /api/servicerequests/{id}
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ServiceRequestResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateServiceRequest(int id, [FromBody] UpdateServiceRequestRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var serviceRequest = await _serviceRequestService.GetByIdAsync(id);
            if (serviceRequest == null)
                return NotFound();

            serviceRequest.ContractId = request.ContractId;
            serviceRequest.Description = request.Description;
            serviceRequest.CostZAR = request.CostZAR;
            serviceRequest.CostUSD = request.CostUSD;
            serviceRequest.ExchangeRateUsed = request.ExchangeRateUsed;
            serviceRequest.Status = request.Status;

            await _serviceRequestService.UpdateAsync(serviceRequest);

            var updated = await _serviceRequestService.GetByIdAsync(id);
            if (updated == null)
                return NotFound();

            return Ok(ServiceRequestResponseDto.FromEntity(updated));
        }

        // DELETE /api/servicerequests/{id}
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteServiceRequest(int id)
        {
            var serviceRequest = await _serviceRequestService.GetByIdAsync(id);
            if (serviceRequest == null)
                return NotFound();

            await _serviceRequestService.DeleteAsync(id);
            return NoContent();
        }
    }
}
