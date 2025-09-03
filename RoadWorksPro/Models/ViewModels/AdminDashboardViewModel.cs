using RoadWorksPro.Models.Entities;

namespace RoadWorksPro.Models.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalOrders { get; set; }
        public int NewOrders { get; set; }
        public int TotalProducts { get; set; }
        public int TotalServices { get; set; }
        public List<RoadOrder> RecentOrders { get; set; } = new();
    }
}