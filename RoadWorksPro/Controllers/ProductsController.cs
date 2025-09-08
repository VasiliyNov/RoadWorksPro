using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Services;

namespace RoadWorksPro.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public ProductsController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index(string? category = null)
        {
            var query = _context.Products.Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
                ViewBag.CurrentCategory = category;
            }

            var products = await query.OrderBy(p => p.Name).ToListAsync();

            ViewBag.Cart = _cartService.GetCart();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            try
            {
                var product = await _context.Products.FindAsync(id);
                if (product == null)
                {
                    return Json(new { success = false, message = "Товар не знайдено" });
                }

                _cartService.AddToCart(product.Id, product.Name, product.Price);
                var cartCount = _cartService.GetCart().TotalItems;

                return Json(new
                {
                    success = true,
                    message = "Товар додано до кошика",
                    cartCount = cartCount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Помилка при додаванні товару" });
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

            if (product == null)
            {
                return NotFound();
            }

            // Get related products from the same category first, then others
            var relatedFromCategory = await _context.Products
                .Where(p => p.Id != id && p.Category == product.Category && p.IsActive)
                .Take(4)
                .ToListAsync();

            var count = relatedFromCategory.Count;

            // If not enough products in the same category, add random from other categories
            if (count < 4)
            {
                var otherProducts = await _context.Products
                    .Where(p => p.Id != id && p.Category != product.Category && p.IsActive)
                    .ToListAsync();

                var additionalProducts = otherProducts
                    .OrderBy(p => Guid.NewGuid())
                    .Take(4 - count)
                    .ToList();

                relatedFromCategory.AddRange(additionalProducts);
            }

            ViewBag.RelatedProducts = relatedFromCategory;
            ViewBag.Cart = _cartService.GetCart();

            return View(product);
        }
    }
}