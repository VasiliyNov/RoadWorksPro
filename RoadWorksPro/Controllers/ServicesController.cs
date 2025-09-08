using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;

namespace RoadWorksPro.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .Where(s => s.IsActive)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(services);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.IsActive);

            if (service == null)
            {
                return NotFound();
            }

            // Get other services for navigation - FIX: Load to memory first
            var otherServices = await _context.Services
                .Where(s => s.Id != id && s.IsActive)
                .ToListAsync(); // Load to memory first

            // Now randomize or order in memory (depends on your needs)
            ViewBag.OtherServices = otherServices
                .OrderBy(s => Guid.NewGuid())
                .Take(3)
                .ToList();

            return View(service);
        }

    }
}