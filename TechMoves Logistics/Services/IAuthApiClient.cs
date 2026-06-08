using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public interface IAuthApiClient
    {
        // POST /api/auth/login
        Task<ApiLoginResponse?> LoginAsync(string username, string password);
    }
}
