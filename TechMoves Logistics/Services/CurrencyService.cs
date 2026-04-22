using System.Text.Json;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Services
{
    public class CurrencyService : ICurrencyService
    {

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public CurrencyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var url = _configuration["CurrencyApi:BaseUrl"];
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                // ExchangeRate-API response format: { "conversion_rates": { "ZAR": 18.5 } }
                var rate = doc.RootElement
                    .GetProperty("conversion_rates")
                    .GetProperty("ZAR")
                    .GetDecimal();

                return rate;
            }
            catch (Exception)
            {
                //Fallback rate if API is down
                return 18.50m;
            }
        }
        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            if (usdAmount < 0)
                throw new ArgumentException("USD amount cannot be negative.");

            if (rate <= 0)
                throw new ArgumentException("Exchange rate must be greater than zero.");

            return Math.Round(usdAmount * rate, 2);
        }
    }
}
