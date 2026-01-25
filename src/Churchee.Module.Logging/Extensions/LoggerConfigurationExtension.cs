using Churchee.Module.Logging.Registrations;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;

namespace Churchee.Module.Logging.Extensions
{
    internal static class LoggerConfigurationExtension
    {
        internal static LoggerConfiguration AddTelegramSinkIfConfigured(this LoggerConfiguration loggerConfiguration, IConfiguration config)
        {
            var telegramSection = config.GetSection("Logging:Telegram");
            bool telegramEnabled = telegramSection.GetValue("Enabled", false);
            string botToken = telegramSection.GetValue<string>("BotToken");
            string chatId = telegramSection.GetValue<string>("ChatId");
            string minimumLevelName = telegramSection.GetValue("MinimumLevel", "Error");

            if (telegramEnabled && !string.IsNullOrWhiteSpace(botToken) && !string.IsNullOrWhiteSpace(chatId))
            {
                if (!Enum.TryParse<LogEventLevel>(minimumLevelName, true, out var telegramMinLevel))
                {
                    telegramMinLevel = LogEventLevel.Error;
                }

                var telegramSink = new TelegramSink(botToken.Trim(), chatId.Trim());

                loggerConfiguration = loggerConfiguration.WriteTo.Sink(telegramSink, restrictedToMinimumLevel: telegramMinLevel);
            }

            return loggerConfiguration;
        }
    }
}
