using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Logging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Logging;
using Serilog.Sinks.MSSqlServer;
using System;
namespace Churchee.Data.EntityFramework
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 5000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();


            var sinkOpts = new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true, AutoCreateSqlDatabase = true };

            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.MSSqlServer(
                    connectionString: config.GetConnectionString("LogsConnection"),
                    sinkOptions: sinkOpts,
                    restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
            .CreateLogger();

            serviceCollection.AddSingleton<ILoggerProvider>(new SerilogLoggerProvider(logger, false));

            serviceCollection.Configure<LoggerFilterOptions>(options =>
            {
                options.AddFilter<SerilogLoggerProvider>(null, LogLevel.Trace);
            });

            serviceCollection.AddDbContext<LogsDBContext>(options => options.UseSqlServer(config.GetConnectionString("LogsConnection")));

        }
    }
}
