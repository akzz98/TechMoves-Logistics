using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMovesLogistics.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/currency")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;

        public CurrencyController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        // GET /api/currency/usd-zar
        [HttpGet("usd-zar")]
        [ProducesResponseType(typeof(CurrencyRateResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsdToZarRate()
        {
            var rate = await _currencyService.GetUsdToZarRateAsync();

            return Ok(new CurrencyRateResponseDto
            {
                FromCurrency = "USD",
                ToCurrency = "ZAR",
                Rate = rate
            });
        }
    }
}
