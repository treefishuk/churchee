using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Helpers;
using Churchee.Common.Storage;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAdminServicesActions(this IServiceCollection services)
        {
            services.RegisterAllTypes<IConfigureAdminServicesAction>(ServiceLifetime.Singleton);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var startUpActions = serviceProvider.GetServices<IConfigureAdminServicesAction>().OrderBy(a => a.Priority);

            foreach (var action in startUpActions)
            {
                action.Execute(services, serviceProvider);

                serviceProvider = services.BuildServiceProvider();
            }
        }

        public static void AddSiteServicesActions(this IServiceCollection services)
        {
            services.RegisterAllTypes<IConfigureSiteServicesAction>(ServiceLifetime.Singleton);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var startUpActions = serviceProvider.GetServices<IConfigureSiteServicesAction>().OrderBy(a => a.Priority);

            foreach (var action in startUpActions)
            {
                action.Execute(services, serviceProvider);

                serviceProvider = services.BuildServiceProvider();
            }
        }

        public static void RegisterSeedActions(this IServiceCollection services)
        {
            services.RegisterAllTypes<ISeedData>(ServiceLifetime.Scoped);
        }

        public static void RunSeedActions(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            using var scope = serviceProvider.CreateScope();

            var seedData = serviceProvider.GetServices<ISeedData>();

            var storage = serviceProvider.GetService<IDataStore>();

            foreach (var data in seedData.OrderBy(o => o.Order))
            {
                data.SeedData(storage);
            }
        }

        public static void RegisterAllTypes<T>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.Scan(
                x =>
                {
                    x.FromAssemblies(AssemblyResolution.GetAssemblies())
                        .AddClasses(classes => classes.AssignableTo(typeof(T)))
                            .AsImplementedInterfaces()
                            .WithLifetime(lifetime);
                });
        }
    }
}
