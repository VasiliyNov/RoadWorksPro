using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class RoadProduct
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string? Dimensions { get; set; } // e.g., "700x700 мм"

        [MaxLength(100)]
        public string? Material { get; set; } // e.g., "Оцинкована сталь"

        [MaxLength(100)]
        public string? Standard { get; set; } // e.g., "ДСТУ 4100-2002"

        [MaxLength(500)]
        public string? ImagePath { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = "other";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
