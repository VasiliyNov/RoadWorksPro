using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Models.ViewModels
{
    public class HomeViewModel
    {
        public List<RoadService> Services { get; set; } = new();
        public List<RoadProduct> FeaturedProducts { get; set; } = new();
        public List<CompanyClient> Clients { get; set; } = new();
        public List<PortfolioItem> PortfolioItems { get; set; } = new();
    }

    public class CompanyClient
    {
        public string Name { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
    }
}