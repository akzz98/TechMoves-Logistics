using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMovesLogistics.Api.Dtos;

namespace TechMovesLogistics.Tests.Integration
{
    [Collection("Integration")]
    public class ClientsApiIntegrationTests
    {
        private readonly ApiWebApplicationFactory _factory;

        public ClientsApiIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // POST /api/clients — 201 Created
        [Fact]
        public async Task PostClient_Returns201_Created()
        {
            var client = await CreateAuthenticatedClientAsync();

            var request = new CreateClientRequestDto
            {
                Name = "Integration Test Client",
                ContactDetails = "client@test.com",
                Region = "Western Cape"
            };

            var response = await client.PostAsJsonAsync("/api/clients", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ClientResponseDto>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(request.Name, created.Name);
            Assert.Equal(request.ContactDetails, created.ContactDetails);
            Assert.Equal(request.Region, created.Region);
        }

        // GET /api/clients/{id} — 404 when not found
        [Fact]
        public async Task GetClientById_WhenNotFound_Returns404()
        {
            var client = await CreateAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/clients/99999");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
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
