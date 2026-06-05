using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class CreateServiceRequestRequestDto
    {
        [Required]
        public int ContractId { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public decimal? CostUSD { get; set; }

        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        public ServiceRequest ToEntity() => new()
        {
            ContractId = ContractId,
            Description = Description,
            CostUSD = CostUSD,
            Status = Status,
            CreatedAt = DateTime.UtcNow
        };
    }
}
