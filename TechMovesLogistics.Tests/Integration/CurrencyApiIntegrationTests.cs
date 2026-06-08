using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMovesLogistics.Api.Dtos;

namespace TechMovesLogistics.Tests.Integration
{
    [Collection("Integration")]
    public class CurrencyApiIntegrationTests
    {
        private readonly ApiWebApplicationFactory _factory;

        public CurrencyApiIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // GET /api/currency/usd-zar — 200 with positive rate
        [Fact]
        public async Task GetUsdToZarRate_Returns200_WithPositiveRate()
        {
            var client = await CreateAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/currency/usd-zar");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var rate = await response.Content.ReadFromJsonAsync<CurrencyRateResponseDto>();
            Assert.NotNull(rate);
            Assert.Equal("USD", rate.FromCurrency);
            Assert.Equal("ZAR", rate.ToCurrency);
            Assert.True(rate.Rate > 0);
        }

        private async Task<HttpClient> CreateAuthenticatedClientAsync()
        {
            var client = _factory.CreateClient();

            var loginResponse = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin@123"
            });

            loginResponse.EnsureSuccessStatusCode();

            var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(loginResult);

            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", loginResult.Token);

            return client;
        }
    }
}
