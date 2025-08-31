using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public RoadOrder Order { get; set; } = null!;

        public int ProductId { get; set; }
        public RoadProduct Product { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal Price { get; set; } // Price at the moment of order

        public decimal Total => Quantity * Price;
    }
}
