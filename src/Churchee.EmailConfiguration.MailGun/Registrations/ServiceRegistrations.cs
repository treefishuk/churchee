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

            serviceCollection.AddHttpClient("Mailgun", client =>
            {
                // Grab values from the configuration
                var apiKey = options.APIKey;
                var base64Auth = Convert.ToBase64String(Encoding.ASCII.GetBytes($"api:{apiKey}"));
                var domain = options.Domain;

                // Set default values on the HttpClient
                client.BaseAddress = new Uri($"https://api.mailgun.net/v3/{domain}/messages");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
            });
        }
    }
}
