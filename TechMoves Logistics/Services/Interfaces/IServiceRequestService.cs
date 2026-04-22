using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services.Interfaces
{
    public interface IServiceRequestService
    {
        Task<IEnumerable<ServiceRequest>> GetAllAsync();
        Task<ServiceRequest?> GetByIdAsync(int id);
        Task CreateServiceRequestAsync(ServiceRequest serviceRequest);
        Task UpdateAsync(ServiceRequest serviceRequest);
        Task DeleteAsync(int id);
    }
}
