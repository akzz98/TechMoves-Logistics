using System.ComponentModel.DataAnnotations;
using TechMoves_Logistics.Models;

namespace TechMovesLogistics.Api.Dtos
{
    public class CreateClientRequestDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Region { get; set; } = string.Empty;

        public Client ToEntity() => new()
        {
            Name = Name,
            ContactDetails = ContactDetails,
            Region = Region
        };
    }
}
