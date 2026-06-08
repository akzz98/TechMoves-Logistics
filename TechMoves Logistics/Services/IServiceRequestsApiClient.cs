using TechMoves_Logistics.Models;

namespace TechMoves_Logistics.Services
{
    public interface IServiceRequestsApiClient
    {
        // GET /api/servicerequests
        Task<IReadOnlyList<ServiceRequest>> GetAllAsync();

        // GET /api/servicerequests/{id}
        Task<ServiceRequest?> GetByIdAsync(int id);

        // POST /api/servicerequests
        Task<ServiceRequest> CreateAsync(ServiceRequest serviceRequest);

        // PUT /api/servicerequests/{id}
        Task<ServiceRequest?> UpdateAsync(int id, ServiceRequest serviceRequest);

        // DELETE /api/servicerequests/{id}
        Task<bool> DeleteAsync(int id);
    }
}
