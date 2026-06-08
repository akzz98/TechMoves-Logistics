using System.Net.Http.Json;

namespace TechMoves_Logistics.Services
{
    public class CurrencyApiClient : ICurrencyApiClient
    {
        private readonly HttpClient _httpClient;

        public CurrencyApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(ApiHttpClientNames.TechMovesApi);
        }

        // GET /api/currency/usd-zar
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            var response = await _httpClient.GetAsync("/api/currency/usd-zar");
            await response.EnsureApiSuccessAsync();
            var rateResponse = await response.Content.ReadFromJsonAsync<CurrencyRateApiResponse>();
            return rateResponse?.Rate
                ?? throw new ApiClientException(
                    System.Net.HttpStatusCode.InternalServerError,
                    "Unable to retrieve USD/ZAR exchange rate.");
        }

        public decimal ConvertUsdToZar(decimal usdAmount, decimal rate)
        {
            if (usdAmount < 0)
                throw new ArgumentException("USD amount cannot be negative.");

            if (rate <= 0)
                throw new ArgumentException("Exchange rate must be greater than zero.");

            return Math.Round(usdAmount * rate, 2);
        }

        private sealed class CurrencyRateApiResponse
        {
            public string FromCurrency { get; set; } = string.Empty;
            public string ToCurrency { get; set; } = string.Empty;
            public decimal Rate { get; set; }
        }
    }
}
