using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Churchee.Data.EntityFramework.Site.Registrations
{
    public class EntityRegistration : IConfigureSiteServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            ArgumentNullException.ThrowIfNull(serviceProvider);
            ArgumentNullException.ThrowIfNull(serviceCollection);

            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("EntityRegistration");

            serviceCollection.RegisterAllTypes<IFrontEndEntityRegistration>(ServiceLifetime.Singleton);

            logger.LogInformation("Registered Implementations of IEntityRegistration");
        }
    }
}
