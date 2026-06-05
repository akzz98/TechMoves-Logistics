using TechMoves_Logistics.Models;

namespace TechMovesLogistics.Api.Dtos
{
    public class ClientResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactDetails { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;

        public static ClientResponseDto FromEntity(Client client) => new()
        {
            Id = client.Id,
            Name = client.Name,
            ContactDetails = client.ContactDetails,
            Region = client.Region
        };
    }
}
