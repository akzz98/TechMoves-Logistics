using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public interface IClientsApiClient
    {
        Task<IReadOnlyList<Client>> GetAllAsync();
        Task<Client?> GetByIdAsync(int id);
        Task<Client> CreateAsync(Client client);
        Task<Client?> UpdateAsync(int id, Client client);
        Task<bool> DeleteAsync(int id);
    }
}
