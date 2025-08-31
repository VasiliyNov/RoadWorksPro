using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class RoadService
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Description { get; set; }

        [MaxLength(100)]
        public string? PriceInfo { get; set; } // e.g., "від 500 грн/м²"

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
