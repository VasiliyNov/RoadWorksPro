using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoadWorksPro.Data;
using RoadWorksPro.Models.Entities;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<AdminUser> _userManager;
        private readonly SignInManager<AdminUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<AdminUser> userManager,
            SignInManager<AdminUser> signInManager,
            ApplicationDbContext context,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _logger = logger;
        }

        // Dashboard
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel
            {
                TotalOrders = await _context.Orders.CountAsync(),
                NewOrders = await _context.Orders.Where(o => o.Status == Models.Enums.OrderStatus.New).CountAsync(),
                TotalProducts = await _context.Products.CountAsync(),
                TotalServices = await _context.Services.CountAsync(),
                RecentOrders = await _context.Orders
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(5)
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // Login GET
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // If already logged in, redirect to dashboard
            if (User.Identity?.IsAuthenticated == true && User.IsInRole("Admin"))
            {
                return RedirectToAction(nameof(Index));
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // Login POST
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user != null && user.IsActive)
                {
                    var result = await _signInManager.PasswordSignInAsync(
                        user.UserName ?? model.Email,
                        model.Password,
                        model.RememberMe,
                        lockoutOnFailure: true);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Admin {model.Email} logged in.");

                        // Update last login
                        user.LastLoginAt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);

                        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }

                        return RedirectToAction(nameof(Index));
                    }

                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Акаунт заблоковано через багато невдалих спроб. Спробуйте пізніше.");
                        return View(model);
                    }
                }

                ModelState.AddModelError(string.Empty, "Невірний email або пароль.");
            }

            return View(model);
        }

        // Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Admin logged out.");
            return RedirectToAction("Index", "Home");
        }

        // Access Denied
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}