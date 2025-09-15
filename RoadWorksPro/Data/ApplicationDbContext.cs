using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Data
{
    public class ApplicationDbContext : IdentityDbContext<AdminUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<RoadProduct> Products { get; set; }

        public DbSet<RoadService> Services { get; set; }

        public DbSet<RoadOrder> Orders { get; set; }

        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<PortfolioItem> PortfolioItems { get; set; }

        public DbSet<PortfolioImage> PortfolioImages { get; set; }

        public DbSet<RoadClient> Clients { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision
            builder.Entity<RoadProduct>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            builder.Entity<RoadOrder>()
                .Property(o => o.TotalAmount)
                .HasPrecision(10, 2);

            builder.Entity<OrderItem>()
                .Property(i => i.Price)
                .HasPrecision(10, 2);

            // Configure relationships
            builder.Entity<OrderItem>()
                .HasOne(i => i.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(i => i.Product)
                .WithMany()
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // PortfolioItem to PortfolioImage relationship
            builder.Entity<PortfolioImage>()
                .HasOne(i => i.PortfolioItem)
                .WithMany(p => p.AdditionalImages)
                .HasForeignKey(i => i.PortfolioItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
