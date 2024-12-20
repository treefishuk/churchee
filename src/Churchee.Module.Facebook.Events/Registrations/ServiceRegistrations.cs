using Churchee.Common.Abstractions.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Facebook.Events.Registrations
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 200;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var facebookApiUrl = config.GetSection("Facebook").GetValue<string>("Api");

            serviceCollection.AddHttpClient("Facebook", client => { client.BaseAddress = new Uri(facebookApiUrl); });
        }
    }
}
