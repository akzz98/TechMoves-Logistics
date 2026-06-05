using System.ComponentModel.DataAnnotations;

namespace TechMoves_Logistics.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(200)]
        public string ContactDetails { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string Region { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
