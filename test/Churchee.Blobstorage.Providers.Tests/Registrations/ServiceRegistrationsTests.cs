using Churchee.Blobstorage.Providers.Azure;
using Churchee.Blobstorage.Providers.Registrations;
using Churchee.Common.Storage;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Blobstorage.Providers.Tests.Registrations
{
    public class ServiceRegistrationsTests
    {
        [Fact]
        public void Priority_ShouldBe1000()
        {
            // Act
            var action = new ServiceRegistrations();

            // Assert
            action.Priority.Should().Be(1000);
        }

        [Fact]
        public void Execute_ShouldRegisterBlobStore()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(s => s.GetSection("ConnectionStrings")["Storage"]).Returns("NotReal");
            services.AddScoped(_ => mockConfiguration.Object);

            var action = new ServiceRegistrations();

            // Act
            action.Execute(services, serviceProvider);
            serviceProvider = services.BuildServiceProvider();

            // Assert
            var processor = serviceProvider.GetService<IBlobStore>();
            processor.Should().NotBeNull();
            processor.Should().BeOfType<AzureBlobStore>();
        }
    }
}
