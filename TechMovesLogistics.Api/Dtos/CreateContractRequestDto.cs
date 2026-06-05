using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class CreateContractRequestDto
    {
        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public ContractStatus Status { get; set; } = ContractStatus.Draft;

        [Required, MaxLength(200)]
        public string ServiceLevel { get; set; } = string.Empty;

        public Contract ToEntity() => new()
        {
            ClientId = ClientId,
            StartDate = StartDate,
            EndDate = EndDate,
            Status = Status,
            ServiceLevel = ServiceLevel
        };
    }
}
