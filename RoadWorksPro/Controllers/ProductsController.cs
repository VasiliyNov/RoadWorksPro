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

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            ViewBag.Cart = _cartService.GetCart();
            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _cartService.AddToCart(product.Id, product.Name, product.Price);

            return Json(new
            {
                success = true,
                message = "Товар додано до кошика",
                cartCount = _cartService.GetCart().TotalItems
            });
        }
    }
}