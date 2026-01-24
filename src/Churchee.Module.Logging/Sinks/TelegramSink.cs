using Serilog.Core;
using Serilog.Events;
using System.Globalization;
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

                string timestamp = logEvent.Timestamp.UtcDateTime.ToString("o", CultureInfo.InvariantCulture);
                string level = logEvent.Level.ToString();
                string message = RenderMessage(logEvent);

                // Compose HTML-encoded message to use parse_mode = HTML
                var fullMessage = new StringBuilder();
                fullMessage.Append("<b>").Append(WebUtility.HtmlEncode(level)).Append("</b>");
                fullMessage.Append(' ');
                fullMessage.Append("<i>").Append(WebUtility.HtmlEncode(timestamp)).Append("</i>");
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

        private static string RenderMessage(LogEvent logEvent)
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