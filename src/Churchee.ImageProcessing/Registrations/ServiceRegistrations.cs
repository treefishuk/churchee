using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.ImageProcessing.Registrations
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<IImageProcessor, DefaultImageProcessor>();
        }
    }
}
