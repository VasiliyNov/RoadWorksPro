using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;

namespace RoadWorksPro.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PortfolioController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? category = null)
        {
            var query = _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .Where(p => p.IsActive)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
                ViewBag.CurrentCategory = category;
            }

            var items = await query
                .OrderBy(p => p.DisplayOrder)
                .ThenByDescending(p => p.CompletedDate)
                .ToListAsync();

            return View(items);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.PortfolioItems
                .Include(p => p.AdditionalImages)
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (item == null)
            {
                return NotFound();
            }

            // Get related works
            ViewBag.RelatedWorks = await _context.PortfolioItems
                .Where(p => p.Id != id && p.Category == item.Category && p.IsActive)
                .OrderBy(p => Guid.NewGuid())
                .Take(3)
                .ToListAsync();

            return View(item);
        }
    }
}