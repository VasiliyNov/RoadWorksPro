using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class PortfolioItem
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(2000)]
        public string? Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } = "other"; // road-marking, asphalt, signs, other

        [Required]
        [MaxLength(500)]
        public string MainImagePath { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? WorkVolume { get; set; } // "1500 м²"

        [MaxLength(200)]
        public string? Materials { get; set; }

        public DateTime CompletedDate { get; set; } = DateTime.UtcNow;

        public CardSize DisplaySize { get; set; } = CardSize.Normal;

        public int DisplayOrder { get; set; }

        public bool IsFeatured { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<PortfolioImage> AdditionalImages { get; set; } = new List<PortfolioImage>();
    }

    public class PortfolioImage
    {
        public int Id { get; set; }

        public int PortfolioItemId { get; set; }
        public PortfolioItem PortfolioItem { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        public string ImagePath { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Caption { get; set; }

        public int DisplayOrder { get; set; }
    }

    public enum CardSize
    {
        Normal = 0,   // 1x1
        Large = 1,    // 2x1 (wide)
        Tall = 2,     // 1x2 (tall)
        Featured = 3  // 2x2 (big)
    }
}