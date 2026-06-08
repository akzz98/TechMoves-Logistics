using System.Net.Http.Json;
using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _httpClient;

        public AuthApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // POST /api/auth/login
        public async Task<ApiLoginResponse?> LoginAsync(string username, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", new
            {
                username,
                password
            });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<ApiLoginResponse>();
        }
    }
}
