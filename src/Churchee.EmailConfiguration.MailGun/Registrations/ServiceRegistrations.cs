using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Exceptions;
using Churchee.EmailConfiguration.MailGun.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http.Headers;
using System.Text;

namespace Churchee.EmailConfiguration.MailGun.Registrations
{
    public class ServiceRegistrations : IConfigureServicesAction
    {
        public int Priority => 200;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            var options = config.GetSection("MailGunOptions").Get<MailGunOptions>() ?? throw new MissingConfirgurationSettingException("MailGunOptions Not Found");

            serviceCollection.AddHttpClient("MailGun", client =>
            {
                // Grab values from the configuration
                var apiKey = options.APIKey;
                var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
                // Set default values on the HttpClient
                client.BaseAddress = new Uri($"{options.BaseUrl}{options.Domain}/messages");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
            });
        }
    }
}
