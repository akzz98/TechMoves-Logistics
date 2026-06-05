using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class UpdateServiceRequestRequestDto
    {
        [Required]
        public int ContractId { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal CostZAR { get; set; }

        public decimal? CostUSD { get; set; }

        public decimal? ExchangeRateUsed { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; }
    }
}
