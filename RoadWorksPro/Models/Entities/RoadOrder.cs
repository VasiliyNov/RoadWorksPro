using RoadWorksPro.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class RoadOrder
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string CustomerPhone { get; set; } = string.Empty;

        [EmailAddress]
        [MaxLength(100)]
        public string? CustomerEmail { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }

        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.New;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation property
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
