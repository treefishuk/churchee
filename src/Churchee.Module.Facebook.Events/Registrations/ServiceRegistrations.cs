using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Facebook.Events.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 200;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var facebookApiUrl = config.GetSection("Facebook").GetValue<string>("Api");

            if (string.IsNullOrEmpty(facebookApiUrl))
            {
                throw new MissingConfirgurationSettingException("Facebook API configuration is missing. Please ensure 'Facebook:Api' is set in the configuration.");
            }

            serviceCollection.AddHttpClient("Facebook", client => { client.BaseAddress = new Uri(facebookApiUrl); });
        }
    }
}
