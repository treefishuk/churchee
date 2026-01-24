using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.MSSqlServer;
using System;

namespace Churchee.Module.Logging.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 5000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var sinkOptions = new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true, AutoCreateSqlDatabase = true };

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MSSqlServer(
                    connectionString: config.GetConnectionString("LogsConnection"),
                    sinkOptions: sinkOptions,
                    restrictedToMinimumLevel: LogEventLevel.Warning)
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information);

            loggerConfiguration = AddTelegramSinkIfConfigured(config, loggerConfiguration);

            var logger = loggerConfiguration.CreateLogger();

            serviceCollection.AddSingleton<ILoggerProvider>(new SerilogLoggerProvider(logger, false));

            serviceCollection.Configure<LoggerFilterOptions>(options =>
            {
                options.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);
            });

            serviceCollection.AddDbContext<LogsDBContext>(options => options.UseSqlServer(config.GetConnectionString("LogsConnection")), ServiceLifetime.Transient);
        }

        private static LoggerConfiguration AddTelegramSinkIfConfigured(IConfiguration config, LoggerConfiguration loggerConfiguration)
        {
            var telegramSection = config.GetSection("Logging:Telegram");
            bool telegramEnabled = telegramSection.GetValue<bool>("Enabled", false);
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
