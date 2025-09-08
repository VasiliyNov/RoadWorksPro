using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoadWorksPro.Models.ViewModels;
using RoadWorksPro.Services;

namespace RoadWorksPro.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Settings")]
    public class AdminSettingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILogger<AdminSettingsController> _logger;

        public AdminSettingsController(
            IConfiguration configuration,
            IEmailService emailService,
            ILogger<AdminSettingsController> logger)
        {
            _configuration = configuration;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            var model = new SettingsViewModel
            {
                SmtpHost = _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                SmtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                SmtpUser = _configuration["Email:SmtpUser"] ?? "",
                SenderName = _configuration["Email:SenderName"] ?? "РоадПро",
                SenderEmail = _configuration["Email:SenderEmail"] ?? "noreply@roadpro.ua",
                AdminEmails = _configuration["Email:AdminNotificationEmails"] ?? "vakawelli@gmail.com",
                TelegramBotToken = _configuration["Telegram:BotToken"] ?? "",
                TelegramChatId = _configuration["Telegram:ChatId"] ?? ""
            };

            return View("~/Views/Admin/Settings/Index.cshtml", model);
        }

        [HttpPost("TestEmail")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TestEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Вкажіть email для тестування";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _emailService.SendTestEmailAsync(email);
                TempData["Success"] = $"Тестовий email відправлено на {email}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email");
                TempData["Error"] = $"Помилка відправки: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}