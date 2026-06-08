using System.Net;
using System.Net.Http.Json;
using TechMovesLogistics.Api.Dtos;

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

        // POST /api/auth/login — 200 with valid credentials
        [Fact]
        public async Task PostLogin_WithValidCredentials_Returns200_WithToken()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Username = "admin",
                Password = "Admin@123"
            });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
            Assert.Equal("admin", result.Username);
        }

        // POST /api/auth/login — 401 with invalid credentials
        [Fact]
        public async Task PostLogin_WithInvalidCredentials_Returns401()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequestDto
            {
                Username = "admin",
                Password = "WrongPassword"
            });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
