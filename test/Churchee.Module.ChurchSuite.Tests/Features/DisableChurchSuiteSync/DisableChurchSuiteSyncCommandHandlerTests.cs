using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Features.Commands.DisableChurchSuiteSync;
using Moq;

namespace Churchee.Module.ChurchSuite.Tests.Features.DisableChurchSuiteSync
{
    public class DisableChurchSuiteSyncCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Disable_Sync()
        {
            // Arrange
            var command = new DisableChurchSuiteSyncCommand();

            var settingStoreMock = new Mock<ISettingStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var jobServiceMock = new Mock<IJobService>();

            var cut = new DisableChurchSuiteSyncCommandHandler(settingStoreMock.Object, currentUserMock.Object, jobServiceMock.Object);

            // Act
            await cut.Handle(command, CancellationToken.None);

            // Assert
            settingStoreMock.Verify(x => x.ClearSetting(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Once);
        }

    }
}
