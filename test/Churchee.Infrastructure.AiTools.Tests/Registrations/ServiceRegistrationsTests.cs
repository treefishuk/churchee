using Churchee.Common.Abstractions.Utilities;
using Churchee.Infrastructure.AiTools.Registrations;
using Churchee.Infrastructure.AiTools.Utilities;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.Infrastructure.AiTools.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void ServiceRegistrations_UseTestAiToolUtilities_False_ReturnsReal()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string?> {
                {"UseTestAiToolUtilities", "false"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            var serviceProvider = BuildServices(configuration);

            // Assert
            serviceProvider.GetRequiredService<IAiToolUtilities>().Should().BeOfType<AiToolUtilities>();
        }


        [Fact]
        public void ServiceRegistrations_UseTestAiToolUtilities_Empty_ReturnsReal()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string?>();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            var serviceProvider = BuildServices(configuration);

            // Assert
            serviceProvider.GetRequiredService<IAiToolUtilities>().Should().BeOfType<AiToolUtilities>();
        }

        [Fact]
        public void ServiceRegistrations_UseTestAiToolUtilities_True_ReturnsTest()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string?> {
                {"UseTestAiToolUtilities", "true"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act
            var serviceProvider = BuildServices(configuration);

            // Assert
            serviceProvider.GetRequiredService<IAiToolUtilities>().Should().BeOfType<TestAiToolUtilities>();
        }

        private static ServiceProvider BuildServices(IConfiguration configuration)
        {
            // Arrange
            var services = new ServiceCollection();

            services.AddSingleton(configuration);

            var serviceProvider = services.BuildServiceProvider();

            var serviceRegistrations = new ServiceRegistrations();

            // Act
            serviceRegistrations.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
