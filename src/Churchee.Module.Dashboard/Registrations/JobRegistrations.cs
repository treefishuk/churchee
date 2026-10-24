using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Dashboard.Jobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Dashboard.Registrations
{
    public class JobRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<ClearOldPageViews>();
            serviceCollection.AddScoped<PopulateAnalyticsJob>();

            var jobService = serviceProvider.GetRequiredService<IJobService>();

            jobService.ScheduleJob<ClearOldPageViews>($"ClearOldPageViews", x => x.ExecuteAsync(CancellationToken.None), Cron.Daily);
            jobService.ScheduleJob<PopulateAnalyticsJob>($"PopulateAnalyticsJob", x => x.ExecuteAsync(CancellationToken.None), Cron.Hourly);
        }
    }
}
