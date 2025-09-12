using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace RoadWorksPro.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task SendOrderNotificationAsync(string orderDetails);
        Task SendTestEmailAsync(string to);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress(
                    _configuration["Email:SenderName"] ?? "LineUp",
                    _configuration["Email:SenderEmail"] ?? "noreply@roadpro.ua"
                ));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(isHtml ? TextFormat.Html : TextFormat.Plain)
                {
                    Text = body
                };

                using var smtp = new SmtpClient();

                // Connect to SMTP server
                await smtp.ConnectAsync(
                    _configuration["Email:SmtpHost"] ?? "smtp.gmail.com",
                    int.Parse(_configuration["Email:SmtpPort"] ?? "587"),
                    SecureSocketOptions.StartTls
                );

                // Authenticate
                await smtp.AuthenticateAsync(
                    _configuration["Email:SmtpUser"],
                    _configuration["Email:SmtpPassword"]
                );

                // Send email
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {to}");
                throw;
            }
        }

        public async Task SendOrderNotificationAsync(string orderDetails)
        {
            var adminEmails = _configuration["Email:AdminNotificationEmails"]?.Split(',')
                ?? new[] { "vakawelli@gmail.com" };

            var subject = "Нове замовлення на сайті LineUp";

            foreach (var adminEmail in adminEmails)
            {
                try
                {
                    await SendEmailAsync(adminEmail.Trim(), subject, orderDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send order notification to {adminEmail}");
                }
            }
        }

        public async Task SendTestEmailAsync(string to)
        {
            var subject = "Тестове повідомлення - LineUp";
            var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2>Тестове повідомлення</h2>
                    <p>Вітаємо! Email налаштовано правильно.</p>
                    <p>Час відправлення (UTC): {DateTime.UtcNow:dd.MM.yyyy HH:mm:ss}</p>
                    <p>Час відправлення (Київ): {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm:ss}</p>
                    <hr>
                    <p><small>LineUp - Система управління</small></p>
                </body>
                </html>
            ";

            await SendEmailAsync(to, subject, body);
        }
    }
}