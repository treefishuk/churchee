using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Logging.Jobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Module.Logging.Registrations
{
    public class JobRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<ClearLogsJob>();

            var jobService = serviceProvider.GetRequiredService<IJobService>();

            jobService.ScheduleJob<ClearLogsJob>($"ClearLogs", x => x.ExecuteAsync(), Cron.Monthly);
        }
    }
}
