using Churchee.Common.Abstractions.Utilities;

namespace Churchee.Presentation.Admin.Registrations
{
    public class LoggerEmailService : IEmailService
    {
        private readonly ILogger<LoggerEmailService> _logger;

        public LoggerEmailService(ILogger<LoggerEmailService> logger)
        {
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlMessage, string plainTextMessage)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                await Task.Run(() => _logger.LogInformation("Email sent with HTML message: {HtmlMessage}", htmlMessage));
            }
        }
    }
}
