using TechMoveLogistics.Repositories.Interfaces;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Repositories.Interfaces;
using TechMoves_Logistics.Services.Interfaces;

namespace TechMoves_Logistics.Services
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly IServiceRequestRepository _serviceRequestRepository;
        private readonly IContractRepository _contractRepository;

        public ServiceRequestService(
            IServiceRequestRepository serviceRequestRepository,
            IContractRepository contractRepository)
        {
            _serviceRequestRepository = serviceRequestRepository;
            _contractRepository = contractRepository;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
            => await _serviceRequestRepository.GetAllAsync();
        public async Task<ServiceRequest?> GetByIdAsync(int id)
            => await _serviceRequestRepository.GetByIdAsync(id);
        public async Task CreateServiceRequestAsync(ServiceRequest serviceRequest)
        {
            //Validate contract status before creating
            var contract = await _contractRepository.GetByIdAsync(serviceRequest.ContractId);

            if (contract == null)
                throw new InvalidOperationException("Contract not found.");

            if (contract.Status == ContractStatus.Expired)
                throw new InvalidOperationException("Cannot create a Service Request for an Expired contract.");

            if (contract.Status == ContractStatus.OnHold)
                throw new InvalidOperationException("Cannot create a Service Request for a contract that is On Hold.");

            await _serviceRequestRepository.AddAsync(serviceRequest);
        }
        public async Task UpdateAsync(ServiceRequest serviceRequest)
            => await _serviceRequestRepository.UpdateAsync(serviceRequest);
        public async Task DeleteAsync(int id)
            => await _serviceRequestRepository.DeleteAsync(id);
    }
}
