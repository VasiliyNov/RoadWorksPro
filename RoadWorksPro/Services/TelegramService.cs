using System.Text;
using System.Net.Http;
using Newtonsoft.Json;

namespace RoadWorksPro.Services
{
    public interface ITelegramService
    {
        Task SendMessageAsync(string message);
        Task SendOrderNotificationAsync(int orderId, string customerName, string phone, decimal total);
        Task SendServiceRequestNotificationAsync(string name, string phone, string service, string message);
    }

    public class TelegramService : ITelegramService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TelegramService> _logger;
        private readonly HttpClient _httpClient;
        private readonly HashSet<string> _allowedChatIds;

        public TelegramService(IConfiguration configuration, ILogger<TelegramService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = new HttpClient();

            // Load allowed chat IDs for security
            var chatIds = _configuration["Telegram:ChatIds"]?.Split(',') ?? Array.Empty<string>();
            _allowedChatIds = new HashSet<string>(chatIds.Select(id => id.Trim()));
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                var botToken = _configuration["Telegram:BotToken"];

                if (string.IsNullOrEmpty(botToken) || !_allowedChatIds.Any())
                {
                    _logger.LogWarning("Telegram not configured properly");
                    return;
                }

                // Send to all allowed chats
                foreach (var chatId in _allowedChatIds)
                {
                    await SendToChat(botToken, chatId, message);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Telegram message");
            }
        }

        private async Task SendToChat(string botToken, string chatId, string message)
        {
            try
            {
                var url = $"https://api.telegram.org/bot{botToken}/sendMessage";

                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("chat_id", chatId),
                    new KeyValuePair<string, string>("text", message),
                    new KeyValuePair<string, string>("parse_mode", "HTML"),
                    new KeyValuePair<string, string>("disable_web_page_preview", "true")
                });

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Failed to send to chat {chatId}: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending to chat {chatId}");
            }
        }

        public async Task SendOrderNotificationAsync(int orderId, string customerName, string phone, decimal total)
        {
            var message = $@"
🛒 <b>НОВЕ ЗАМОВЛЕННЯ #{orderId}</b>
━━━━━━━━━━━━━━━━━━
👤 <b>Клієнт:</b> {customerName}
📞 <b>Телефон:</b> <code>{phone}</code>
💰 <b>Сума:</b> {total:N2} грн
🕒 <b>Час:</b> {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}
━━━━━━━━━━━━━━━━━━
📋 <a href='https://yourdomain.com/Admin/Orders/Details/{orderId}'>Переглянути деталі</a>
";

            await SendMessageAsync(message);
        }

        public async Task SendServiceRequestNotificationAsync(string name, string phone, string service, string message)
        {
            var telegramMessage = $@"
🔧 <b>ЗАЯВКА НА ПОСЛУГУ</b>
━━━━━━━━━━━━━━━━━━
👤 <b>Ім'я:</b> {name}
📞 <b>Телефон:</b> <code>{phone}</code>
🛠 <b>Послуга:</b> {service ?? "Не вказано"}
💬 <b>Повідомлення:</b> {message ?? "—"}
🕒 <b>Час:</b> {DateTime.UtcNow.AddHours(2):dd.MM.yyyy HH:mm}
━━━━━━━━━━━━━━━━━━
⚡ Зв'яжіться з клієнтом протягом 15 хвилин!
";

            await SendMessageAsync(telegramMessage);
        }
    }
}