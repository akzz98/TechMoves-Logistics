using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMoves_Logistics.Services.Interfaces
{
    public interface IContractService
    {
        Task<IEnumerable<Contract>> GetAllContractsAsync();
        Task<Contract?> GetContractByIdAsync(int id);
        Task<IEnumerable<Contract>> SearchContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status);
        Task CreateContractAsync(Contract contract);
        Task UpdateContractAsync(Contract contract);
        Task DeleteContractAsync(int id);
    }
}
