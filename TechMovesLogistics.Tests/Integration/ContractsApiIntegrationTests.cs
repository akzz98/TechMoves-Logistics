using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using TechMovesLogistics.Api.Dtos;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Tests.Integration
{
    public class ContractsApiIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly ApiWebApplicationFactory _factory;

        public ContractsApiIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
        }

        // GET /api/contracts
        [Fact]
        public async Task GetContracts_Returns200_WithJsonBody()
        {
            var client = await CreateAuthenticatedClientAsync();

            var response = await client.GetAsync("/api/contracts");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(JsonValueKind.Array, json.ValueKind);
        }

        // GET /api/contracts?status=
        [Fact]
        public async Task GetContracts_WithStatusFilter_ReturnsMatchingContracts()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            await CreateTestContractAsync(
                client, clientId, ContractStatus.OnHold,
                new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
            await CreateTestContractAsync(
                client, clientId, ContractStatus.Expired,
                new DateTime(2025, 1, 1), new DateTime(2025, 12, 31));

            var response = await client.GetAsync($"/api/contracts?status={ContractStatus.OnHold}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractResponseDto>>();
            Assert.NotNull(contracts);
            Assert.Single(contracts);
            Assert.Equal(ContractStatus.OnHold, contracts[0].Status);
        }

        // GET /api/contracts?startDate=&endDate=
        [Fact]
        public async Task GetContracts_WithDateRangeFilter_ReturnsMatchingContracts()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            await CreateTestContractAsync(
                client, clientId, ContractStatus.Draft,
                new DateTime(2026, 6, 15), new DateTime(2026, 12, 31));
            await CreateTestContractAsync(
                client, clientId, ContractStatus.Draft,
                new DateTime(2024, 1, 1), new DateTime(2024, 12, 31));

            var response = await client.GetAsync(
                "/api/contracts?startDate=2026-06-01&endDate=2026-12-31");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var contracts = await response.Content.ReadFromJsonAsync<List<ContractResponseDto>>();
            Assert.NotNull(contracts);
            Assert.Single(contracts);
            Assert.True(contracts[0].StartDate >= new DateTime(2026, 1, 1));
            Assert.True(contracts[0].EndDate <= new DateTime(2026, 12, 31));
        }

        // POST /api/contracts
        [Fact]
        public async Task PostContract_Returns201_Created()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var request = new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2026, 7, 1),
                EndDate = new DateTime(2027, 6, 30),
                Status = ContractStatus.Draft,
                ServiceLevel = "Premium"
            };

            var response = await client.PostAsJsonAsync("/api/contracts", request);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var contract = await response.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(contract);
            Assert.True(contract.Id > 0);
            Assert.Equal(clientId, contract.ClientId);
            Assert.Equal(request.StartDate, contract.StartDate);
            Assert.Equal(request.EndDate, contract.EndDate);
            Assert.Equal(ContractStatus.Draft, contract.Status);
            Assert.Equal("Premium", contract.ServiceLevel);

            Assert.NotNull(response.Headers.Location);
            Assert.Contains($"/api/contracts/{contract.Id}", response.Headers.Location.ToString());
        }

        // POST then GET /api/contracts/{id} — data integrity
        [Fact]
        public async Task PostContract_ThenGetById_ReturnsPersistedData()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var request = new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2026, 8, 1),
                EndDate = new DateTime(2027, 7, 31),
                Status = ContractStatus.Active,
                ServiceLevel = "Enterprise"
            };

            var createResponse = await client.PostAsJsonAsync("/api/contracts", request);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(created);

            var getResponse = await client.GetAsync($"/api/contracts/{created.Id}");

            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = await getResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(fetched);
            Assert.Equal(created.Id, fetched.Id);
            Assert.Equal(clientId, fetched.ClientId);
            Assert.Equal(request.StartDate, fetched.StartDate);
            Assert.Equal(request.EndDate, fetched.EndDate);
            Assert.Equal(ContractStatus.Active, fetched.Status);
            Assert.Equal("Enterprise", fetched.ServiceLevel);
        }

        // PATCH /api/contracts/{id}/status
        [Fact]
        public async Task PatchContractStatus_Returns200_WithUpdatedStatus()
        {
            var client = await CreateAuthenticatedClientAsync();
            var clientId = await CreateTestClientAsync(client);

            var createResponse = await client.PostAsJsonAsync("/api/contracts", new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = new DateTime(2026, 3, 1),
                EndDate = new DateTime(2026, 12, 31),
                Status = ContractStatus.Draft,
                ServiceLevel = "Standard"
            });

            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(created);
            Assert.Equal(ContractStatus.Draft, created.Status);

            var patchResponse = await client.PatchAsJsonAsync(
                $"/api/contracts/{created.Id}/status",
                new UpdateContractStatusRequestDto { Status = ContractStatus.OnHold });

            Assert.Equal(HttpStatusCode.OK, patchResponse.StatusCode);

            var updated = await patchResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(updated);
            Assert.Equal(created.Id, updated.Id);
            Assert.Equal(ContractStatus.OnHold, updated.Status);

            var getResponse = await client.GetAsync($"/api/contracts/{created.Id}");
            var fetched = await getResponse.Content.ReadFromJsonAsync<ContractResponseDto>();
            Assert.NotNull(fetched);
            Assert.Equal(ContractStatus.OnHold, fetched.Status);
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
                Name = "Filter Test Client",
                ContactDetails = "filter@test.com",
                Region = "Gauteng"
            });

            response.EnsureSuccessStatusCode();
            var created = await response.Content.ReadFromJsonAsync<ClientResponseDto>();
            return created!.Id;
        }

        private static async Task CreateTestContractAsync(
            HttpClient client,
            int clientId,
            ContractStatus status,
            DateTime startDate,
            DateTime endDate)
        {
            var response = await client.PostAsJsonAsync("/api/contracts", new CreateContractRequestDto
            {
                ClientId = clientId,
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                ServiceLevel = "Standard"
            });

            response.EnsureSuccessStatusCode();
        }
    }
}