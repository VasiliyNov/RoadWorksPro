using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Services
{
    public interface ICartService
    {
        CartViewModel GetCart();

        void AddToCart(int productId, string productName, decimal price);

        void RemoveFromCart(int productId);

        void UpdateQuantity(int productId, int quantity);

        void ClearCart();
    }

    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string CartSessionKey = "ShoppingCart";

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext!.Session;

        public CartViewModel GetCart()
        {
            var cartJson = Session.GetString(CartSessionKey);
            if (string.IsNullOrEmpty(cartJson))
            {
                return new CartViewModel();
            }

            return JsonConvert.DeserializeObject<CartViewModel>(cartJson) ?? new CartViewModel();
        }

        public void AddToCart(int productId, string productName, decimal price)
        {
            var cart = GetCart();

            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = 1
                });
            }

            SaveCart(cart);
        }

        public void RemoveFromCart(int productId)
        {
            var cart = GetCart();
            cart.Items.RemoveAll(x => x.ProductId == productId);
            SaveCart(cart);
        }

        public void UpdateQuantity(int productId, int quantity)
        {
            if (quantity <= 0)
            {
                RemoveFromCart(productId);
                return;
            }

            var cart = GetCart();
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
        }

        public void ClearCart()
        {
            Session.Remove(CartSessionKey);
        }

        private void SaveCart(CartViewModel cart)
        {
            Session.SetString(CartSessionKey, JsonConvert.SerializeObject(cart));
        }
    }
}