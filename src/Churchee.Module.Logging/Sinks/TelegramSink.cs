using Serilog.Core;
using Serilog.Events;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Registrations
{
    public class TelegramSink : ILogEventSink
    {
        private readonly HttpClient _httpClient;
        private readonly string _sendMessageUrl;
        private readonly string _chatId;

        public TelegramSink(string botToken, string chatId, HttpClient httpClient = null)
        {
            // botToken expected without "bot" prefix; API endpoint requires "bot{token}"
            _sendMessageUrl = string.Format(CultureInfo.InvariantCulture, "https://api.telegram.org/bot{0}/sendMessage", botToken);
            _chatId = chatId;
            _httpClient = httpClient ?? new HttpClient();
        }

        public void Emit(LogEvent logEvent)
        {
            if (logEvent == null)
            {
                return;
            }

            try
            {
                // Only send for Error and Fatal by default (the logger configuration will also filter),
                // but double-check levels here to avoid unnecessary work.
                if (logEvent.Level < LogEventLevel.Error)
                {
                    return;
                }

                string timestamp = logEvent.Timestamp.UtcDateTime.ToString("dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                string level = logEvent.Level.ToString();
                string username = logEvent.Properties.FirstOrDefault(w => w.Key == "Username").Value?.ToString() ?? "Unknown User";
                string tenantName = logEvent.Properties.FirstOrDefault(w => w.Key == "Tenant").Value?.ToString() ?? "Unknown Tenant";
                string message = RenderMessage(logEvent);

                // Compose HTML-encoded message to use parse_mode = HTML
                var fullMessage = new StringBuilder();
                fullMessage.Append("<b>User: </b>")
                           .Append(WebUtility.HtmlEncode(username));
                fullMessage.AppendLine();
                fullMessage.Append("<b>Tenant: </b>")
                           .Append(WebUtility.HtmlEncode(tenantName));
                fullMessage.AppendLine();
                fullMessage.Append("<b>Type: </b>")
                            .Append(WebUtility.HtmlEncode(level));
                fullMessage.AppendLine();
                fullMessage.Append("<b>Time: </b>")
                            .Append(WebUtility.HtmlEncode(timestamp));
                fullMessage.AppendLine();
                fullMessage.AppendLine();
                fullMessage.Append(WebUtility.HtmlEncode(message));

                if (logEvent.Exception != null)
                {
                    fullMessage.AppendLine();
                    fullMessage.AppendLine();
                    fullMessage.Append("<pre>");
                    fullMessage.Append(WebUtility.HtmlEncode(logEvent.Exception.ToString()));
                    fullMessage.Append("</pre>");
                }

                var payload = new
                {
                    chat_id = _chatId,
                    text = fullMessage.ToString(),
                    parse_mode = "HTML",
                    disable_web_page_preview = true
                };

                _ = SendFireAndForgetAsync(payload);
            }
            catch
            {
                // Swallow everything - logging must not throw.
            }
        }

        internal virtual Task SendFireAndForgetAsync(object payload)
        {
            return Task.Run(() => SendPostAsync(payload));
        }

        internal async Task SendPostAsync(object payload)
        {
            try
            {
                string json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                using var response = await _httpClient.PostAsync(_sendMessageUrl, content).ConfigureAwait(false);
                // Intentionally ignore response content; only ensure exceptions are caught.
            }
            catch
            {
                // Swallow any exceptions to avoid impacting application flow.
            }
        }

        internal static string RenderMessage(LogEvent logEvent)
        {
            try
            {
                // Use the RenderedMessage if available
                if (logEvent.RenderMessage() is string rendered && !string.IsNullOrWhiteSpace(rendered))
                {
                    return rendered;
                }

                // Fallback to message template
                return logEvent.MessageTemplate?.Text ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}