using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Facebook.Events.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Facebook.Events.Registrations
{
    public class JobRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<SyncFacebookEventsJob>();
        }
    }
}
