using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Tests.Integration
{
    [Collection("Integration")]
    public class ServiceRequestsApiIntegrationTests
    {
        private readonly ApiWebApplicationFactory _factory;

        public ServiceRequestsApiIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // POST /api/servicerequests — blocked when contract is Expired
        [Fact]
        public async Task PostServiceRequest_ForExpiredContract_Returns400()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var contractResponse = await client.PostAsJsonAsync("/api/contracts", new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2024, 1, 1),
                EndDate = new DateTime(2024, 12, 31),
                Status = ContractStatus.Expired,
                ServiceLevel = "Standard"
            });

            contractResponse.EnsureSuccessStatusCode();
            var contract = await contractResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(contract);

            var response = await client.PostAsJsonAsync("/api/servicerequests", new CreateServiceRequestRequestDto
            {
                ContractId = contract.Id,
                Description = "Delivery attempt on expired contract",
                CostUSD = 100m
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var message = await response.Content.ReadAsStringAsync();
            Assert.Contains("Expired", message, StringComparison.OrdinalIgnoreCase);
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

        private static async Task<int> CreateTestClientAsync(HttpClient client)
        {
            var response = await client.PostAsJsonAsync("/api/clients", new CreateClientRequestDto
            {
                Name = "Service Request Test Client",
                ContactDetails = "sr@test.com",
                Region = "Gauteng"
            });

            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<ClientResponseDto>();
            return created!.Id;
        }
    }
}
