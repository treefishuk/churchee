using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Churchee.Data.EntityFramework
{
    public class EntityRegistration : IConfigureServicesAction
    {
        public int Priority => 1000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger("EntityRegistration");

            serviceCollection.RegisterAllTypes<IEntityRegistration>(ServiceLifetime.Singleton);

            logger.LogInformation("Registered Imlementations of IEntityRegistration");
        }
    }
}
