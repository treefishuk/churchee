using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Hangfire.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Module.Hangfire.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 200;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            serviceCollection.AddHangfire((provider, config) =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .CreateDatabaseIfNotExists(configuration.GetConnectionString("HangfireConnection"))

                .UseFilter(new AutomaticRetryAttribute
                {
                    Attempts = 3,          // or 0 to disable retries
                    DelaysInSeconds = [10, 30, 60] // optional custom delays
                })
                .UseSqlServerStorage(configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                });
            });

            serviceCollection.AddScoped<IJobService, JobService>();

            if (configuration.GetSection("Hangfire")["IsService"] != "true")
            {
                serviceCollection.AddHangfireServer();

            }
        }
    }
}
