using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class UpdateContractRequestDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public ContractStatus Status { get; set; }

        [Required, MaxLength(200)]
        public string ServiceLevel { get; set; } = string.Empty;

        public void ApplyTo(Contract contract)
        {
            contract.ClientId = ClientId;
            contract.StartDate = StartDate;
            contract.EndDate = EndDate;
            contract.Status = Status;
            contract.ServiceLevel = ServiceLevel;
        }
    }
}
