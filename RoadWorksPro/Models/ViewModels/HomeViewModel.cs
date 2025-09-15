using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<RoadService> Services { get; set; } = new();
        public List<RoadProduct> FeaturedProducts { get; set; } = new();
        public List<RoadClient> Clients { get; set; } = new();
        public List<PortfolioItem> PortfolioItems { get; set; } = new();
    }
}