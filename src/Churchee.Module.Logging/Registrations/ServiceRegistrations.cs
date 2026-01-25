using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Logging.Extensions;
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
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .AddTelegramSinkIfConfigured(config);

            var logger = loggerConfiguration.CreateLogger();

            serviceCollection.AddSingleton<ILoggerProvider>(new SerilogLoggerProvider(logger, false));

            serviceCollection.Configure<LoggerFilterOptions>(options =>
            {
                options.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);
            });

            serviceCollection.AddDbContext<LogsDBContext>(options => options.UseSqlServer(config.GetConnectionString("LogsConnection")), ServiceLifetime.Transient);
        }


    }
}
