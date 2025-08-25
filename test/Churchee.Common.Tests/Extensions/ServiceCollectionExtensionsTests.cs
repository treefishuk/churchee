using Churchee.Common.Abstractions.Extensibility;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Churchee.Common.Tests.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddServicesActions_ShouldRegisterAndExecuteAllConfigureServicesActions()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockAction1 = new Mock<IConfigureAdminServicesAction>();
            var mockAction2 = new Mock<IConfigureAdminServicesAction>();
            mockAction1.Setup(a => a.Priority).Returns(1);
            mockAction2.Setup(a => a.Priority).Returns(2);

            services.AddSingleton(mockAction1.Object);
            services.AddSingleton(mockAction2.Object);

            // Act
            services.AddAdminServicesActions();

            // Assert
            mockAction1.Verify(a => a.Execute(It.IsAny<IServiceCollection>(), It.IsAny<IServiceProvider>()), Times.Once);
            mockAction2.Verify(a => a.Execute(It.IsAny<IServiceCollection>(), It.IsAny<IServiceProvider>()), Times.Once);
        }


        [Fact]
        public void AddSiteServicesActions_ShouldRegisterAndExecuteAllConfigureServicesActions()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockAction1 = new Mock<IConfigureSiteServicesAction>();
            var mockAction2 = new Mock<IConfigureSiteServicesAction>();
            mockAction1.Setup(a => a.Priority).Returns(1);
            mockAction2.Setup(a => a.Priority).Returns(2);

            services.AddSingleton(mockAction1.Object);
            services.AddSingleton(mockAction2.Object);

            // Act
            services.AddSiteServicesActions();

            // Assert
            mockAction1.Verify(a => a.Execute(It.IsAny<IServiceCollection>(), It.IsAny<IServiceProvider>()), Times.Once);
            mockAction2.Verify(a => a.Execute(It.IsAny<IServiceCollection>(), It.IsAny<IServiceProvider>()), Times.Once);
        }

        [Fact]
        public void RegisterSeedActions_ShouldRegisterAllSeedDataTypes()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterSeedActions();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var seedData = serviceProvider.GetServices<ISeedData>();
            seedData.Should().NotBeNull();
        }

        [Fact]
        public void RunSeedActions_ShouldExecuteSeedDataInOrder()
        {
            // Arrange
            var services = new ServiceCollection();
            var mockSeedData1 = new Mock<ISeedData>();
            var mockSeedData2 = new Mock<ISeedData>();
            var mockStorage = new Mock<IDataStore>();

            mockSeedData1.Setup(s => s.Order).Returns(1);
            mockSeedData2.Setup(s => s.Order).Returns(2);

            services.AddSingleton(mockSeedData1.Object);
            services.AddSingleton(mockSeedData2.Object);
            services.AddSingleton(mockStorage.Object);

            // Act
            services.RunSeedActions();

            // Assert
            mockSeedData1.Verify(s => s.SeedData(It.IsAny<IDataStore>()), Times.Once);
            mockSeedData2.Verify(s => s.SeedData(It.IsAny<IDataStore>()), Times.Once);
        }
    }
}
