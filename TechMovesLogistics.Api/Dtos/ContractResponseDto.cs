using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;

namespace TechMovesLogistics.Api.Dtos
{
    public class ContractResponseDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public string ServiceLevel { get; set; } = string.Empty;
        public string? SignedAgreementPath { get; set; }
        public ClientSummaryDto? Client { get; set; }

        public static ContractResponseDto FromEntity(Contract contract) => new()
        {
            Id = contract.Id,
            ClientId = contract.ClientId,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status,
            ServiceLevel = contract.ServiceLevel,
            SignedAgreementPath = contract.SignedAgreementPath,
            Client = contract.Client == null ? null : new ClientSummaryDto
            {
                Id = contract.Client.Id,
                Name = contract.Client.Name,
                Region = contract.Client.Region
            }
        };
    }

    public class ClientSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
    }
}
