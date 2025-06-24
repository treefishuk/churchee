using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.X.Features.Tweets.Commands;
using Moq;

namespace Churchee.Module.X.Tests.Features.Tweets.Commands.DisableTweetsSync
{
    public class DisableTweetsSyncCommandHandlerTests
    {
        private readonly Mock<IJobService> _jobServiceMock = new();
        private readonly Mock<IDataStore> _dataStoreMock = new();
        private readonly Mock<ISettingStore> _settingStoreMock = new();
        private readonly Mock<IRepository<Token>> _tokenRepoMock = new();

        private readonly DisableTweetsSyncCommandHandler _handler;

        public DisableTweetsSyncCommandHandlerTests()
        {
            _dataStoreMock.Setup(ds => ds.GetRepository<Token>()).Returns(_tokenRepoMock.Object);
            _handler = new DisableTweetsSyncCommandHandler(_jobServiceMock.Object, _dataStoreMock.Object, _settingStoreMock.Object);
        }

        [Fact]
        public async Task Handle_RemovesScheduledJob_AndClearsSettings_AndDeletesToken_IfTokenExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DisableTweetsSyncCommand(tenantId);

            var token = new Token(tenantId, Churchee.Module.x.Helpers.SettingKeys.XBearerToken, string.Empty);

            var tokens = new[] { token }.AsQueryable();

            _tokenRepoMock.Setup(r => r.GetQueryable()).Returns(tokens);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _jobServiceMock.Verify(j => j.RemoveScheduledJob($"{tenantId}_SyncTweets"), Times.Once);
            _settingStoreMock.Verify(s => s.ClearSetting(It.IsAny<Guid>(), tenantId), Times.Exactly(2));
            _tokenRepoMock.Verify(r => r.PermanentDelete(token), Times.Once);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_DoesNotDeleteToken_IfTokenDoesNotExist()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var command = new DisableTweetsSyncCommand(tenantId);

            _tokenRepoMock.Setup(r => r.GetQueryable()).Returns(Enumerable.Empty<Token>().AsQueryable());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            _tokenRepoMock.Verify(r => r.PermanentDelete(It.IsAny<Token>()), Times.Never);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
    }

}
