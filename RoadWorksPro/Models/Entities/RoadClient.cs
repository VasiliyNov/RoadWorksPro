using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class RoadClient
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? LogoPath { get; set; }

        [MaxLength(500)]
        public string? WebsiteUrl { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}