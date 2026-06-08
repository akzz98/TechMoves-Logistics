using System.Net;

namespace TechMovesLogistics.Tests.Integration
{
    [Collection("Integration")]
    public class AuthApiIntegrationTests
    {
        private readonly ApiWebApplicationFactory _factory;

        public AuthApiIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // GET /api/contracts — 401 without JWT
        [Fact]
        public async Task GetContracts_WithoutToken_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
