using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Videos.Entities;
using Churchee.Module.YouTube.Features.YouTube.Commands;
using Churchee.Module.YouTube.Helpers;
using Moq;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.DisableYouTubeSync
{
    public class DisableYouTubeSyncCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Clears_SettingsAnd_Saves()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var cmd = new DisableYouTubeSyncCommand();

            var settingStore = new Mock<ISettingStore>();
            settingStore.Setup(s => s.AddOrUpdateSetting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            settingStore.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(string.Empty);

            var currentUser = new Mock<ICurrentUser>();
            currentUser.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var videoRepo = new Mock<IRepository<Video>>();
            videoRepo.Setup(r => r.Create(It.IsAny<Video>()));

            var dataStore = new Mock<IDataStore>();
            dataStore.Setup(d => d.GetRepository<Video>()).Returns(videoRepo.Object);
            dataStore.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var jobService = new Mock<IJobService>();

            var handlerInstance = new DisableYouTubeSyncCommandHandler(settingStore.Object, currentUser.Object, jobService.Object, dataStore.Object);

            // Act
            var result = await handlerInstance.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            // Settings Are Cleared
            settingStore.Verify(j => j.ClearSetting(SettingKeys.Handle, tenantId), Times.Once);
            settingStore.Verify(j => j.ClearSetting(SettingKeys.ChannelId, tenantId), Times.Once);
            settingStore.Verify(j => j.ClearSetting(SettingKeys.Playlist, tenantId), Times.Once);
            dataStore.Verify(j => j.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);


        }

    }
}
