using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMoves_Logistics.Services
{
    public interface IContractsApiClient
    {
        // GET /api/contracts?startDate=&endDate=&status=
        Task<IReadOnlyList<Contract>> SearchAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);

        // GET /api/contracts/{id}
        Task<Contract?> GetByIdAsync(int id);

        // POST /api/contracts
        Task<Contract> CreateAsync(Contract contract);

        // PUT /api/contracts/{id}
        Task<Contract?> UpdateAsync(int id, Contract contract);

        // PATCH /api/contracts/{id}/status
        Task<Contract?> UpdateStatusAsync(int id, ContractStatus status);

        // DELETE /api/contracts/{id}
        Task<bool> DeleteAsync(int id);

        // POST /api/contracts/{id}/agreement
        Task<Contract?> UploadAgreementAsync(int id, IFormFile file);

        // GET /api/contracts/{id}/agreement
        Task<byte[]?> DownloadAgreementAsync(int id);
    }
}
