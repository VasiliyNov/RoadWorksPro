using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.ViewModels;
using RoadWorksPro.Services;

namespace RoadWorksPro.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;

        public CartController(ApplicationDbContext context, ICartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        public IActionResult Index()
        {
            var cart = _cartService.GetCart();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            _cartService.UpdateQuantity(productId, quantity);
            var cart = _cartService.GetCart();

            return Json(new
            {
                success = true,
                cartTotal = cart.Total,
                cartCount = cart.TotalItems,
                itemSubtotal = cart.Items.FirstOrDefault(x => x.ProductId == productId)?.Subtotal ?? 0
            });
        }

        [HttpPost]
        public IActionResult RemoveItem(int productId)
        {
            _cartService.RemoveFromCart(productId);
            var cart = _cartService.GetCart();

            return Json(new
            {
                success = true,
                cartTotal = cart.Total,
                cartCount = cart.TotalItems
            });
        }

        [HttpPost]
        public IActionResult Clear()
        {
            _cartService.ClearCart();
            return RedirectToAction("Index");
        }

        // API endpoint for getting cart count
        [HttpGet]
        public IActionResult GetCartCount()
        {
            var count = _cartService.GetCart().TotalItems;
            return Json(new { count });
        }
    }
}