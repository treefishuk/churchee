using Churchee.Common.Abstractions.Extensibility;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Module.Hangfire.Registrations
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 200;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            serviceCollection.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .CreateDatabaseIfNotExists(config.GetConnectionString("HangfireConnection"))
                .UseSqlServerStorage(config.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true,
                }));


            serviceCollection.AddHangfireServer();

            string connectionString = serviceProvider.GetService<IConfiguration>().GetConnectionString("HangfireConnection");

        }
    }
}
