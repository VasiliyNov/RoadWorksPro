using Microsoft.AspNetCore.Mvc;
using RoadWorksPro.Services;
using RoadWorksPro.Models.ViewModels;

namespace RoadWorksPro.Controllers
{
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ITelegramService _telegramService;
        private readonly ILogger<ContactController> _logger;

        public ContactController(
            IEmailService emailService,
            ITelegramService telegramService,
            ILogger<ContactController> logger)
        {
            _emailService = emailService;
            _telegramService = telegramService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ServiceRequest(ServiceRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Перевірте правильність заповнення форми" });
            }

            try
            {
                // Send Email notification
                var emailBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Нова заявка на послугу</h2>
                        <hr>
                        <p><strong>Ім'я:</strong> {model.Name}</p>
                        <p><strong>Телефон:</strong> {model.Phone}</p>
                        <p><strong>Послуга:</strong> {model.Service ?? "Не вказано"}</p>
                        <p><strong>Повідомлення:</strong> {model.Message ?? "—"}</p>
                        <hr>
                        <p>Час: {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}</p>
                    </body>
                    </html>";

                await _emailService.SendOrderNotificationAsync(emailBody);

                // Send Telegram notification
                await _telegramService.SendServiceRequestNotificationAsync(
                    model.Name,
                    model.Phone,
                    model.Service,
                    model.Message
                );

                return Json(new
                {
                    success = true,
                    message = "Дякуємо за заявку! Ми зв'яжемося з вами протягом 15 хвилин."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing service request");
                return Json(new
                {
                    success = false,
                    message = "Виникла помилка. Будь ласка, зателефонуйте нам."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickContact(string phone)
        {
            if (string.IsNullOrEmpty(phone))
            {
                return Json(new { success = false, message = "Вкажіть номер телефону" });
            }

            try
            {
                // Simple notification for quick contact form
                var message = $@"
📞 <b>ШВИДКА ЗАЯВКА</b>
━━━━━━━━━━━━━━━━━━
📱 <b>Телефон:</b> <code>{phone}</code>
🕒 <b>Час:</b> {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}
━━━━━━━━━━━━━━━━━━
⚡ Передзвоніть клієнту якнайшвидше!
";

                await _telegramService.SendMessageAsync(message);

                // Email notification
                var emailBody = $@"
                    <html>
                    <body>
                        <h2>Швидка заявка</h2>
                        <p><strong>Телефон:</strong> {phone}</p>
                        <p>Час: {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}</p>
                    </body>
                    </html>";

                await _emailService.SendOrderNotificationAsync(emailBody);

                return Json(new
                {
                    success = true,
                    message = "Дякуємо! Ми зателефонуємо вам протягом 15 хвилин."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing quick contact");
                return Json(new { success = false, message = "Помилка відправки" });
            }
        }
    }
}