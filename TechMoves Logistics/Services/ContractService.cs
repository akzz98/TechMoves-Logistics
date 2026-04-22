using TechMoveLogistics.Repositories.Interfaces;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Services
{
    public class ContractService : IContractService
    {
        private readonly IContractRepository _contractRepository;

        public ContractService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<IEnumerable<Contract>> GetAllContractsAsync()
            => await _contractRepository.GetAllAsync();

        public async Task<Contract?> GetContractByIdAsync(int id)
            => await _contractRepository.GetByIdAsync(id);

        public async Task<IEnumerable<Contract>> SearchContractsAsync(DateTime? startDate, DateTime? endDate, ContractStatus? status)
            => await _contractRepository.SearchAsync(startDate, endDate, status);

        public async Task CreateContractAsync(Contract contract)
            => await _contractRepository.AddAsync(contract);

        public async Task UpdateContractAsync(Contract contract)
            => await _contractRepository.UpdateAsync(contract);

        public async Task DeleteContractAsync(int id)
            => await _contractRepository.DeleteAsync(id);
    }
}
