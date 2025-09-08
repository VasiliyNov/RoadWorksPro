using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models;
using RoadWorksPro.Models.ViewModels;
using System.Diagnostics;

namespace RoadWorksPro.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                Services = await _context.Services
                    .Where(s => s.IsActive)
                    .OrderBy(s => s.Id)
                    .ToListAsync(),

                FeaturedProducts = await _context.Products
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.Id)
                    .Take(4)
                    .ToListAsync(),

                PortfolioItems = await _context.PortfolioItems
                    .Where(p => p.IsActive && p.IsFeatured)
                    .OrderBy(p => p.DisplayOrder)
                    .Take(6)
                    .ToListAsync(),

                // Sample clients
                Clients = new List<CompanyClient>
                {
                    new() { Name = "Укравтодор", LogoUrl = "/images/clients/ukravtodor.png" },
                    new() { Name = "Київавтодор", LogoUrl = "/images/clients/kyivavtodor.png" },
                    new() { Name = "WOG", LogoUrl = "/images/clients/wog.png" },
                    new() { Name = "ОККО", LogoUrl = "/images/clients/okko.png" },
                    new() { Name = "Епіцентр", LogoUrl = "/images/clients/epicentr.png" }
                }
            };

            return View(viewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}