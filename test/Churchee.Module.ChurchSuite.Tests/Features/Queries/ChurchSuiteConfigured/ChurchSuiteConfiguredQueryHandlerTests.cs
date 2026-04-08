using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Features.Queries.ChurchSuiteConfigured;
using Moq;

namespace Churchee.Module.ChurchSuite.Tests.Features.Queries.ChurchSuiteConfigured
{
    public class ChurchSuiteConfiguredQueryHandlerTests
    {


        [Fact]
        public async Task Handle_Returns_True_When_Configuration_Valid()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();
            var mockcurrentUser = new Mock<ICurrentUser>();

            mockSettingStore.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync("https://example.churchsuite.com/events/json");

            var handler = new ChurchSuiteConfiguredQueryHandler(mockSettingStore.Object, mockcurrentUser.Object);

            var query = new ChurchSuiteConfiguredQuery();

            // Act
            bool result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_Returns_False_When_Configuration_Invalid()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();
            var mockcurrentUser = new Mock<ICurrentUser>();

            var handler = new ChurchSuiteConfiguredQueryHandler(mockSettingStore.Object, mockcurrentUser.Object);

            var query = new ChurchSuiteConfiguredQuery();

            // Act
            bool result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

    }
}
