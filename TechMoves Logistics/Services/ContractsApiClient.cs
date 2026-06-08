using System.Net.Http.Headers;
using System.Net.Http.Json;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMoves_Logistics.Services
{
    public class ContractsApiClient : IContractsApiClient
    {
        private readonly HttpClient _httpClient;

        public ContractsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(ApiHttpClientNames.TechMovesApi);
        }

        // GET /api/contracts?startDate=&endDate=&status=
        public async Task<IReadOnlyList<Contract>> SearchAsync(
            DateTime? startDate,
            DateTime? endDate,
            ContractStatus? status)
        {
            var queryParts = new List<string>();

            if (startDate.HasValue)
                queryParts.Add($"startDate={startDate.Value:yyyy-MM-dd}");

            if (endDate.HasValue)
                queryParts.Add($"endDate={endDate.Value:yyyy-MM-dd}");

            if (status.HasValue)
                queryParts.Add($"status={status.Value}");

            var url = queryParts.Count > 0
                ? $"/api/contracts?{string.Join("&", queryParts)}"
                : "/api/contracts";

            var contracts = await _httpClient.GetFromJsonAsync<List<Contract>>(url);
            return contracts ?? [];
        }

        // GET /api/contracts/{id}
        public async Task<Contract?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/contracts/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        // POST /api/contracts
        public async Task<Contract> CreateAsync(Contract contract)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/contracts", new
            {
                contract.ClientId,
                contract.StartDate,
                contract.EndDate,
                contract.Status,
                contract.ServiceLevel
            });

            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Contract>())!;
        }

        // PUT /api/contracts/{id}
        public async Task<Contract?> UpdateAsync(int id, Contract contract)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/contracts/{id}", new
            {
                contract.ClientId,
                contract.StartDate,
                contract.EndDate,
                contract.Status,
                contract.ServiceLevel
            });

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        // PATCH /api/contracts/{id}/status
        public async Task<Contract?> UpdateStatusAsync(int id, ContractStatus status)
        {
            var request = new HttpRequestMessage(HttpMethod.Patch, $"/api/contracts/{id}/status")
            {
                Content = JsonContent.Create(new { status })
            };

            var response = await _httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        // DELETE /api/contracts/{id}
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/contracts/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }

        // POST /api/contracts/{id}/agreement
        public async Task<Contract?> UploadAgreementAsync(int id, IFormFile file)
        {
            await using var stream = file.OpenReadStream();
            using var content = new MultipartFormDataContent();
            var fileContent = new StreamContent(stream);

            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"/api/contracts/{id}/agreement", content);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Contract>();
        }

        // GET /api/contracts/{id}/agreement
        public async Task<byte[]?> DownloadAgreementAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/contracts/{id}/agreement");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
