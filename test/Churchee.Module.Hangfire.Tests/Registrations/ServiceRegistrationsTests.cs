using Churchee.Module.Hangfire.Registrations;
using FluentAssertions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Churchee.Module.Hangfire.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        private const string FakeConnectionString = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Fake;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        [Fact]
        public void ServiceRegistrations_WithIsServiceSetToTrue_RegistersHanginterfacesOnly()
        {
            // Arrange
            var services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(s => s.GetSection("Hangfire")["IsService"]).Returns("true");
            configurationMock.Setup(s => s.GetSection("ConnectionStrings")["HangfireConnection"]).Returns(FakeConnectionString);

            services.AddSingleton(configurationMock.Object);

            var serviceProvider = services.BuildServiceProvider();
            var cut = new ServiceRegistrations();

            // Act
            cut.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<IRecurringJobManager>().Should().NotBeNull();
            serviceProvider.GetService<IBackgroundJobClient>().Should().NotBeNull();
            serviceProvider.GetService<IHostedService>().Should().BeNull();
        }

        [Fact]
        public void ServiceRegistrations_Execute_WithIsServiceSetToFalse_RegistersHanginterfacesAndServer()
        {
            // Arrange
            var services = new ServiceCollection();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(s => s.GetSection("Hangfire")["IsService"]).Returns("false");
            configurationMock.Setup(s => s.GetSection("ConnectionStrings")["HangfireConnection"]).Returns(FakeConnectionString);

            services.AddSingleton(configurationMock.Object);

            var serviceProvider = services.BuildServiceProvider();
            var cut = new ServiceRegistrations();

            // Act
            cut.Execute(services, serviceProvider);

            serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<IHostedService>().Should().NotBeNull();
            serviceProvider.GetService<IHostedService>().Should().BeOfType<BackgroundJobServerHostedService>();

        }
    }
}