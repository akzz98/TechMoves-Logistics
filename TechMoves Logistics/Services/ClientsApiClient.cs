using System.Net.Http.Json;
using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public class ClientsApiClient : IClientsApiClient
    {
        private readonly HttpClient _httpClient;

        public ClientsApiClient(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(ApiHttpClientNames.TechMovesApi);
        }

        // GET /api/clients
        public async Task<IReadOnlyList<Client>> GetAllAsync()
        {
            var clients = await _httpClient.GetFromJsonAsync<List<Client>>("/api/clients");
            return clients ?? [];
        }

        // GET /api/clients/{id}
        public async Task<Client?> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"/api/clients/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Client>();
        }

        // POST /api/clients
        public async Task<Client> CreateAsync(Client client)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/clients", new
            {
                client.Name,
                client.ContactDetails,
                client.Region
            });

            response.EnsureSuccessStatusCode();
            return (await response.Content.ReadFromJsonAsync<Client>())!;
        }

        // PUT /api/clients/{id}
        public async Task<Client?> UpdateAsync(int id, Client client)
        {
            var response = await _httpClient.PutAsJsonAsync($"/api/clients/{id}", new
            {
                client.Name,
                client.ContactDetails,
                client.Region
            });

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Client>();
        }

        // DELETE /api/clients/{id}
        public async Task<bool> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/clients/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return false;

            response.EnsureSuccessStatusCode();
            return true;
        }
    }
}
