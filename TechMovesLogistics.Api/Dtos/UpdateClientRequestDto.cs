using System.ComponentModel.DataAnnotations;

namespace TechMovesLogistics.Api.Dtos
{
    public class UpdateClientRequestDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Region { get; set; } = string.Empty;
    }
}
