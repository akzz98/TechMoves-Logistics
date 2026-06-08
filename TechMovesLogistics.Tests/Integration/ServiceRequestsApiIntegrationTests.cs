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

        // POST /api/servicerequests — 201 for Active contract
        [Fact]
        public async Task PostServiceRequest_ForActiveContract_Returns201()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var contractResponse = await client.PostAsJsonAsync("/api/contracts", new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2026, 1, 1),
                EndDate = new DateTime(2026, 12, 31),
                Status = ContractStatus.Active,
                ServiceLevel = "Standard"
            });

            contractResponse.EnsureSuccessStatusCode();
            var contract = await contractResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(contract);

            var response = await client.PostAsJsonAsync("/api/servicerequests", new CreateServiceRequestRequestDto
            {
                ContractId = contract.Id,
                Description = "Scheduled delivery",
                CostUSD = 250m
            });

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var created = await response.Content.ReadFromJsonAsync<ServiceRequestResponseDto>();
            Assert.NotNull(created);
            Assert.True(created.Id > 0);
            Assert.Equal(contract.Id, created.ContractId);
            Assert.Equal("Scheduled delivery", created.Description);
            Assert.True(created.CostZAR > 0);
        }

        // POST /api/servicerequests — 400 when contract is On Hold
        [Fact]
        public async Task PostServiceRequest_ForOnHoldContract_Returns400()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var contractResponse = await client.PostAsJsonAsync("/api/contracts", new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2026, 2, 1),
                EndDate = new DateTime(2026, 11, 30),
                Status = ContractStatus.OnHold,
                ServiceLevel = "Standard"
            });

            contractResponse.EnsureSuccessStatusCode();
            var contract = await contractResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(contract);

            var response = await client.PostAsJsonAsync("/api/servicerequests", new CreateServiceRequestRequestDto
            {
                ContractId = contract.Id,
                Description = "Blocked delivery attempt",
                CostUSD = 50m
            });

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var message = await response.Content.ReadAsStringAsync();
            Assert.Contains("On Hold", message, StringComparison.OrdinalIgnoreCase);
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
