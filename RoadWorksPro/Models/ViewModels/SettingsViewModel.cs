using System.ComponentModel.DataAnnotations;

namespace RoadWorksPro.Models.ViewModels
{
    public class SettingsViewModel
    {
        [Display(Name = "SMTP Сервер")]
        public string SmtpHost { get; set; } = "smtp.gmail.com";

        [Display(Name = "SMTP Порт")]
        public int SmtpPort { get; set; } = 587;

        [Display(Name = "SMTP Користувач")]
        public string SmtpUser { get; set; } = string.Empty;

        [Display(Name = "SMTP Пароль")]
        [DataType(DataType.Password)]
        public string? SmtpPassword { get; set; }

        [Display(Name = "Ім'я відправника")]
        public string SenderName { get; set; } = "LineUp";

        [Display(Name = "Email відправника")]
        [EmailAddress]
        public string SenderEmail { get; set; } = "noreply@roadpro.ua";

        [Display(Name = "Email адміністраторів")]
        public string AdminEmails { get; set; } = string.Empty;

        [Display(Name = "Telegram Bot Token")]
        public string? TelegramBotToken { get; set; }

        [Display(Name = "Telegram Chat ID")]
        public string? TelegramChatId { get; set; }
    }
}