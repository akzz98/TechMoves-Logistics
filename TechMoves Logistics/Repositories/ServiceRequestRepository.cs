using Microsoft.EntityFrameworkCore;
using TechMoves_Logistics.Data;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Repositories.Interfaces;

namespace TechMoves_Logistics.Repositories
{
    public class ServiceRequestRepository : IServiceRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ServiceRequest>> GetAllAsync()
            => await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .ToListAsync();

        public async Task<ServiceRequest?> GetByIdAsync(int id)
            => await _context.ServiceRequests
                .Include(sr => sr.Contract)
                .ThenInclude(c => c!.Client)
                .FirstOrDefaultAsync(sr => sr.Id == id);

        public async Task<IEnumerable<ServiceRequest>> GetByContractIdAsync(int contractId)
            => await _context.ServiceRequests
                .Where(sr => sr.ContractId == contractId)
                .ToListAsync();

        public async Task AddAsync(ServiceRequest serviceRequest)
        {
            await _context.ServiceRequests.AddAsync(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ServiceRequest serviceRequest)
        {
            _context.ServiceRequests.Update(serviceRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var sr = await _context.ServiceRequests.FindAsync(id);
            if (sr != null)
            {
                _context.ServiceRequests.Remove(sr);
                await _context.SaveChangesAsync();
            }
        }
    }
}

