using Churchee.EmailConfiguration.MailGun.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.EmailConfiguration.MailGun.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void ServiceRegistrations_ShouldReturnExpectedServices()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddHttpClient();

            //Arrange
            var inMemorySettings = new Dictionary<string, string?> {
                {"MailGunOptions:APIKey", "test-api-key"},
                {"MailGunOptions:BaseUrl", "https://api.mailgun.net/v3/"},
                {"MailGunOptions:Domain", "example.com"},
                {"MailGunOptions:FromEmail", "test@example.com"},
                {"MailGunOptions:FromName", "Test User"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var serviceRegistrations = new ServiceRegistrations();

            // Act
            serviceRegistrations.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();

            // Assert
            var httpClient = serviceProvider.GetRequiredService<IHttpClientFactory>().CreateClient("MailGun");

            Assert.NotNull(httpClient);
            Assert.Equal(new Uri("https://api.mailgun.net/v3/example.com/messages"), httpClient.BaseAddress);
            Assert.Equal("Basic", httpClient.DefaultRequestHeaders.Authorization.Scheme);
            Assert.NotNull(httpClient.DefaultRequestHeaders.Authorization.Parameter);
        }

    }
}
