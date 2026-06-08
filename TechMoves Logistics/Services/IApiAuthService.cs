using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public interface IApiAuthService
    {
        Task<ApiLoginResponse?> LoginAsync(string username, string password);
    }
}
