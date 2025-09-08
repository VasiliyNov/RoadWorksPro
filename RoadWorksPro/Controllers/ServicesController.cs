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

            // Get other services for navigation
            ViewBag.OtherServices = await _context.Services
                .Where(s => s.Id != id && s.IsActive)
                .OrderBy(s => s.Id)
                .ToListAsync();

            return View(service);
        }
    }
}