using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechMoves_Logistics.Models.Enums;


namespace TechMoves_Logistics.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }

        [Required, MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        // Cost stored in ZAR
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostZAR { get; set; }

        // Original USD amount entered by user
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CostUSD { get; set; }

        // Exchange rate used at time of creation
        [Column(TypeName = "decimal(18,4)")]
        public decimal? ExchangeRateUsed { get; set; }

        [Required]
        public ServiceRequestStatus Status { get; set; } = ServiceRequestStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
