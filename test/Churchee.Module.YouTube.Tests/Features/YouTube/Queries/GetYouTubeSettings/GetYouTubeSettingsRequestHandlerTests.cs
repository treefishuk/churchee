using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.YouTube.Features.YouTube.Queries;
using Churchee.Module.YouTube.Helpers;
using Moq;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Queries
{
    public class GetYouTubeSettingsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_Settings_From_Store_And_LastRun_From_JobService()
        {
            // Arrange
            var expectedTenant = Guid.NewGuid();
            var expectedHandle = "SomeHandle";
            var expectedPage = "VideosPage";
            var expectedLastRun = DateTime.UtcNow;

            var mockCurrentUser = new Mock<ICurrentUser>();
            mockCurrentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(expectedTenant);

            var mockStore = new Mock<ISettingStore>();
            mockStore
                .Setup(s => s.GetSettingValue(SettingKeys.Handle, expectedTenant))
                .ReturnsAsync(expectedHandle);
            mockStore
                .Setup(s => s.GetSettingValue(SettingKeys.VideosPageName, expectedTenant))
                .ReturnsAsync(expectedPage);

            var mockJobService = new Mock<IJobService>();
            mockJobService
                .Setup(j => j.GetLastRunDate(It.IsAny<string>()))
                .Returns(expectedLastRun);

            var handler = new GetYouTubeSettingsRequestHandler(mockStore.Object, mockCurrentUser.Object, mockJobService.Object);

            var request = new GetYouTubeSettingsRequest();

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(expectedHandle, response.Handle);
            Assert.Equal(expectedPage, response.NameForContent);
            Assert.Equal(expectedLastRun, response.LastRun);

            mockJobService.Verify(j => j.GetLastRunDate(It.IsAny<string>()), Times.Once);
        }
    }
}
