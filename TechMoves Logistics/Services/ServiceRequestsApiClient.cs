using System.Net.Http.Json;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMoves_Logistics.Services
{
    public class ServiceRequestsApiClient : IServiceRequestsApiClient
    {
        private readonly HttpClient _httpClient;

        public ServiceRequestsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(ApiHttpClientNames.TechMovesApi);
        }

        // GET /api/servicerequests
        public async Task<IReadOnlyList<ServiceRequest>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("/api/servicerequests");
            await response.EnsureApiSuccessAsync();
            var responses = await response.Content.ReadFromJsonAsync<List<ServiceRequestApiResponse>>();
            return responses?.Select(r => r.ToModel()).ToList() ?? [];
        }

        // GET /api/servicerequests/{id}
        public async Task<ServiceRequest?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/servicerequests/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            await response.EnsureApiSuccessAsync();
            var apiResponse = await response.Content.ReadFromJsonAsync<ServiceRequestApiResponse>();
            return apiResponse?.ToModel();
        }

        // POST /api/servicerequests
        public async Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/servicerequests", new
            {
                serviceRequest.ContractId,
                serviceRequest.Description,
                serviceRequest.CostUSD,
                serviceRequest.Status
            });

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var message = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(string.IsNullOrWhiteSpace(message)
                    ? "Unable to create service request."
                    : message.Trim('"'));
            }

            await response.EnsureApiSuccessAsync();
            var created = await response.Content.ReadFromJsonAsync<ServiceRequestApiResponse>();
            return created!.ToModel();
        }

        // PUT /api/servicerequests/{id}
        public async Task<ServiceRequest?> UpdateAsync(int id, ServiceRequest serviceRequest)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/servicerequests/{id}", new
            {
                serviceRequest.ContractId,
                serviceRequest.Description,
                serviceRequest.CostZAR,
                serviceRequest.CostUSD,
                serviceRequest.ExchangeRateUsed,
                serviceRequest.Status
            });

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            await response.EnsureApiSuccessAsync();
            var updated = await response.Content.ReadFromJsonAsync<ServiceRequestApiResponse>();
            return updated?.ToModel();
        }

        // DELETE /api/servicerequests/{id}
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/servicerequests/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            await response.EnsureApiSuccessAsync();
            return true;
        }

        // Maps API JSON (flat contract/client fields) to ServiceRequest for MVC views.
        private sealed class ServiceRequestApiResponse
        {
            public int Id { get; set; }
            public int ContractId { get; set; }
            public string Description { get; set; } = string.Empty;
            public decimal CostZAR { get; set; }
            public decimal? CostUSD { get; set; }
            public decimal? ExchangeRateUsed { get; set; }
            public ServiceRequestStatus Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public string? ContractServiceLevel { get; set; }
            public string? ClientName { get; set; }

            public ServiceRequest ToModel() => new()
            {
                Id = Id,
                ContractId = ContractId,
                Description = Description,
                CostZAR = CostZAR,
                CostUSD = CostUSD,
                ExchangeRateUsed = ExchangeRateUsed,
                Status = Status,
                CreatedAt = CreatedAt,
                Contract = new Contract
                {
                    Id = ContractId,
                    ServiceLevel = ContractServiceLevel ?? string.Empty,
                    Client = ClientName == null ? null : new Client { Name = ClientName }
                }
            };
        }
    }
}
