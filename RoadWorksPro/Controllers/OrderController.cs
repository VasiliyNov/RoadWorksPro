using Microsoft.AspNetCore.Mvc;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.Enums;
using RoadWorksPro.Models.ViewModels;
using RoadWorksPro.Services;

namespace RoadWorksPro.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderController> _logger;
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;

        public OrderController(
            ApplicationDbContext context,
            ICartService cartService,
            ILogger<OrderController> logger,
            IEmailService emailService,
            ITelegramService telegramService)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;
            _emailService = emailService;
            _telegramService = telegramService;
        }

        public IActionResult Checkout()
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = new CheckoutViewModel
            {
                Cart = cart
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            var cart = _cartService.GetCart();
            if (!cart.Items.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            model.Cart = cart;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Create order
                var order = new RoadOrder
                {
                    CustomerName = model.CustomerName,
                    CustomerPhone = model.CustomerPhone,
                    CustomerEmail = model.CustomerEmail,
                    Comment = model.Comment,
                    TotalAmount = cart.Total,
                    Status = OrderStatus.New,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Add order items
                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    };
                    _context.OrderItems.Add(orderItem);
                }

                await _context.SaveChangesAsync();

                // Send email notification to admin
                try
                {
                    var orderDetailsHtml = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif;'>
                            <h2>Нове замовлення #{order.Id}</h2>
                            <hr>
                            <h3>Інформація про клієнта:</h3>
                            <p><strong>Ім'я:</strong> {order.CustomerName}</p>
                            <p><strong>Телефон:</strong> {order.CustomerPhone}</p>
                            <p><strong>Email:</strong> {order.CustomerEmail ?? "не вказано"}</p>
                            <p><strong>Коментар:</strong> {order.Comment ?? "немає"}</p>
                            
                            <h3>Товари:</h3>
                            <table border='1' cellpadding='5' cellspacing='0'>
                                <tr>
                                    <th>Товар</th>
                                    <th>Кількість</th>
                                    <th>Ціна</th>
                                    <th>Сума</th>
                                </tr>";

                    foreach (var item in cart.Items)
                    {
                        orderDetailsHtml += $@"
                            <tr>
                                <td>{item.ProductName}</td>
                                <td>{item.Quantity}</td>
                                <td>₴ {item.Price:N2}</td>
                                <td>₴ {item.Subtotal:N2}</td>
                            </tr>";
                    }

                    // TODO: Update product links when product pages are ready
                    orderDetailsHtml += $@"
                            </table>
                            <h3>Загальна сума: ₴ {order.TotalAmount:N2}</h3>
                            <hr>
                            <p>Дата замовлення: {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}</p>
                            <p><a href='https://yourdomain.com/Admin/Orders/Details/{order.Id}'>Переглянути в адмін-панелі</a></p>
                        </body>
                        </html>";

                    await _emailService.SendOrderNotificationAsync(orderDetailsHtml);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email notification for order {OrderId}", order.Id);
                    // Don't throw - email failure shouldn't break the order
                }

                // Send Telegram notification
                try
                {
                    await _telegramService.SendOrderNotificationAsync(
                        order.Id,
                        order.CustomerName,
                        order.CustomerPhone,
                        order.TotalAmount
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send Telegram notification");
                }

                // Clear cart
                _cartService.ClearCart();

                TempData["OrderSuccess"] = true;
                TempData["OrderId"] = order.Id;

                return RedirectToAction("Success", new { id = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                ModelState.AddModelError("", "Виникла помилка при оформленні замовлення. Спробуйте ще раз.");
                return View(model);
            }
        }

        public IActionResult Success(int id)
        {
            ViewBag.OrderId = id;
            return View();
        }
    }
}