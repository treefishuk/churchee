using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Queue;
using Churchee.Module.Site.Jobs;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Site.Registration
{
    public class JobRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<PublishArticlesJob>();

            var jobService = serviceProvider.GetRequiredService<IJobService>();

            jobService.ScheduleJob<PublishArticlesJob>($"PublishArticles", x => x.ExecuteAsync(CancellationToken.None), Cron.Daily);
        }
    }
}
