using System.Net.Http.Json;
using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public class ApiAuthService : IApiAuthService
    {
        private readonly HttpClient _httpClient;

        public ApiAuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

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
