using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;

namespace RoadWorksPro.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.ProductCount = await _context.Products.CountAsync();
            ViewBag.ServiceCount = await _context.Services.CountAsync();
            ViewBag.OrderCount = await _context.Orders.CountAsync();
            ViewBag.AdminCount = await _context.Users.CountAsync();

            ViewBag.Products = await _context.Products.Take(3).ToListAsync();
            ViewBag.Services = await _context.Services.ToListAsync();

            return View();
        }
    }
}