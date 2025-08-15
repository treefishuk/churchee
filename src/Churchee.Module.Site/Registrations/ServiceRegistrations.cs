using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Site.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Site
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 5000;
        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<StylesCompilerHelper, StylesCompilerHelper>();
        }
    }
}
