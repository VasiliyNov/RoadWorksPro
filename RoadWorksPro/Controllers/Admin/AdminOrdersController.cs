using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Enums;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Orders")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminOrdersController> _logger;

        public AdminOrdersController(ApplicationDbContext context, ILogger<AdminOrdersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/Orders
        [HttpGet("")]
        public async Task<IActionResult> Index(OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
                ViewBag.CurrentStatus = status.Value;
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Count orders by status for filter badges
            ViewBag.StatusCounts = new
            {
                All = await _context.Orders.CountAsync(),
                New = await _context.Orders.CountAsync(o => o.Status == OrderStatus.New),
                InProgress = await _context.Orders.CountAsync(o => o.Status == OrderStatus.InProgress),
                Completed = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Completed),
                Cancelled = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Cancelled)
            };

            return View("~/Views/Admin/Orders/Index.cshtml", orders);
        }

        // GET: Admin/Orders/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Orders/Details.cshtml", order);
        }

        // POST: Admin/Orders/UpdateStatus/5
        [HttpPost("UpdateStatus/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            var oldStatus = order.Status;
            order.Status = status;
            order.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Order #{id} status updated from {oldStatus} to {status}");

                TempData["Success"] = $"Статус замовлення #{id} оновлено";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating order #{id} status");
                TempData["Error"] = "Помилка при оновленні статусу";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Admin/Orders/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Orders/Delete.cshtml", order);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order #{id} deleted");
                TempData["Success"] = $"Замовлення #{id} видалено";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}