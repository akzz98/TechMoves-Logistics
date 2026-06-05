using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class UpdateContractStatusRequestDto
    {
        [Required]
        public ContractStatus Status { get; set; }
    }
}
