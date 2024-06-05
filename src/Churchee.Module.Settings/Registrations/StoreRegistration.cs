using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Storage;
using Churchee.Module.Settings.Store;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Settings.Registrations
{
    public class StoreRegistration : IConfigureServicesAction
    {
        public int Priority => 5000;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<ISettingStore, SettingStore>();
        }

    }
}
