using Churchee.Common.Exceptions;
using Churchee.Module.Facebook.Events.Registrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Module.Facebook.Events.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void Execute_RegistersFacebookHttpClient_WithConfiguredBaseAddress()
        {
            // Arrange
            var services = new ServiceCollection();
            var facebookApiUrl = "https://graph.facebook.com/";
            var inMemorySettings = new Dictionary<string, string?>
            {
                {"Facebook:Api", facebookApiUrl}
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

            var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient("Facebook");

            // Assert
            Assert.NotNull(httpClient);
            Assert.Equal(new Uri(facebookApiUrl), httpClient.BaseAddress);
        }

        [Fact]
        public void Priority_ShouldBe200()
        {
            // Arrange
            var serviceRegistrations = new ServiceRegistrations();

            // Act & Assert
            Assert.Equal(200, serviceRegistrations.Priority);
        }

        [Fact]
        public void Execute_ThrowsInvalidOperationException_WhenFacebookApiConfigMissing()
        {
            // Arrange
            var services = new ServiceCollection();
            var inMemorySettings = new Dictionary<string, string?>();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            services.AddSingleton(configuration);
            var serviceProvider = services.BuildServiceProvider();

            var serviceRegistrations = new ServiceRegistrations();

            // Act & Assert
            Assert.Throws<MissingConfigurationSettingException>(() =>
                serviceRegistrations.Execute(services, serviceProvider));
        }

    }
}
