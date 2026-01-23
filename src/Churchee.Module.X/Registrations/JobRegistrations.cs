using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.X.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.X.Registrations
{
    public class JobRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<SyncTweets>();
        }
    }
}
