using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Features.Commands;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;

namespace Churchee.Module.Facebook.Events.Tests.Features.Commands
{
    public class DisableFacebookSyncCommandHandlerTests
    {
        private readonly Mock<ISettingStore> _settingStoreMock = new();
        private readonly Mock<IRecurringJobManager> _recurringJobManagerMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<ILogger<DisableFacebookSyncCommandHandler>> _loggerMock = new();
        private readonly Mock<IDataStore> _dataStoreMock = new();
        private readonly Mock<IRepository<Token>> _tokenRepoMock = new();

        private readonly Guid _tenantId = Guid.NewGuid();

        public DisableFacebookSyncCommandHandlerTests()
        {
            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(_tenantId);
            _dataStoreMock.Setup(x => x.GetRepository<Token>()).Returns(_tokenRepoMock.Object);
        }

        [Fact]
        public async Task Handle_Successful_DisablesSyncAndDeletesTokens()
        {
            // Arrange
            var tokens = new[]
            {
                new Token(_tenantId, SettingKeys.FacebookAccessToken, string.Empty),
                new Token(_tenantId, SettingKeys.FacebookPageAccessToken, string.Empty),
            }.AsQueryable();

            _tokenRepoMock.Setup(x => x.GetQueryable()).Returns(tokens);

            var handler = new DisableFacebookSyncCommandHandler(
                _settingStoreMock.Object,
                _recurringJobManagerMock.Object,
                _currentUserMock.Object,
                _loggerMock.Object,
                _dataStoreMock.Object
            );

            var command = new DisableFacebookSyncCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _recurringJobManagerMock.Verify(x => x.RemoveIfExists($"{_tenantId}_FacebookEvents"), Times.Once);
            _settingStoreMock.Verify(x => x.ClearSetting(It.IsAny<Guid>(), _tenantId), Times.Exactly(2));
            _tokenRepoMock.Verify(x => x.PermanentDelete(It.IsAny<Token>()), Times.Exactly(2));
            _dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async Task Handle_Exception_LogsErrorAndAddsErrorToResponse()
        {
            // Arrange
            _recurringJobManagerMock.Setup(x => x.RemoveIfExists(It.IsAny<string>())).Throws(new Exception("fail"));

            var handler = new DisableFacebookSyncCommandHandler(
                _settingStoreMock.Object,
                _recurringJobManagerMock.Object,
                _currentUserMock.Object,
                _loggerMock.Object,
                _dataStoreMock.Object
            );

            var command = new DisableFacebookSyncCommand();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to disable Facebook Sync")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            Assert.False(result.IsSuccess);
            Assert.Single(result.Errors);
            Assert.Equal("Failed to disable Facebook Sync", result.Errors[0].Description);
        }
    }
}
