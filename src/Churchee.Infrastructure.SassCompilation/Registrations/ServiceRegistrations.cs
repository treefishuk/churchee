using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Infrastructure.SassCompilation.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 5000;
        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddSingleton<ISassComplier, SassCliCompiler>();
        }
    }
}
