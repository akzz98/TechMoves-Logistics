using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class ServiceRequestResponseDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal CostZAR { get; set; }
        public decimal? CostUSD { get; set; }
        public decimal? ExchangeRateUsed { get; set; }
        public ServiceRequestStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ContractServiceLevel { get; set; }
        public string? ClientName { get; set; }

        public static ServiceRequestResponseDto FromEntity(ServiceRequest request) => new()
        {
            Id = request.Id,
            ContractId = request.ContractId,
            Description = request.Description,
            CostZAR = request.CostZAR,
            CostUSD = request.CostUSD,
            ExchangeRateUsed = request.ExchangeRateUsed,
            Status = request.Status,
            CreatedAt = request.CreatedAt,
            ContractServiceLevel = request.Contract?.ServiceLevel,
            ClientName = request.Contract?.Client?.Name
        };
    }
}
