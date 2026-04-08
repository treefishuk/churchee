using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.ChurchSuite.Jobs;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.ChurchSuite.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 6000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<SyncChurchSuiteEventsJob>();
        }
    }
}
