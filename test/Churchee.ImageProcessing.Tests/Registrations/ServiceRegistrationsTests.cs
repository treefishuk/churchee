using Churchee.Common.Abstractions.Utilities;
using Churchee.ImageProcessing.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace Churchee.ImageProcessing.Tests.Registrations
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
        public void Execute_ShouldRegisterImageProcessor()
        {
            // Arrange
            var services = new ServiceCollection();
            var serviceProvider = services.BuildServiceProvider();
            var action = new ServiceRegistrations();

            // Act
            action.Execute(services, serviceProvider);
            serviceProvider = services.BuildServiceProvider();

            // Assert
            var processor = serviceProvider.GetService<IImageProcessor>();
            processor.Should().NotBeNull();
            processor.Should().BeOfType<DefaultImageProcessor>();
        }
    }
}
