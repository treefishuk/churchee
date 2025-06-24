namespace Churchee.Module.X.Tests.Features.Tweets.Queries.CheckXIntergrationStatus
{
    using Churchee.Common.Abstractions.Auth;
    using Churchee.Common.Abstractions.Queue;
    using Churchee.Common.Storage;
    using Churchee.Module.X.Features.Tweets.Queries;
    using Churchee.Module.X.Features.Tweets.Queries.CheckXIntergrationStatus;
    using Moq;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    public class CheckXIntegrationStatusQueryHandlerTests
    {
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<ICurrentUser> _currentUser = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Guid _tenantId = Guid.NewGuid();

        private CheckXIntegrationStatusQueryHandler CreateHandler() =>
            new(_settingStore.Object, _currentUser.Object, _jobService.Object);

        [Fact]
        public async Task Handle_ReturnsNotConfigured_WhenUserIdIsNull()
        {
            // Arrange
            _currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(_tenantId);
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), _tenantId)).ReturnsAsync((string?)null);

            var handler = CreateHandler();
            var query = new CheckXIntegrationStatusQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Configured);
            Assert.Null(result.LastRun);
        }

        [Fact]
        public async Task Handle_ReturnsNotConfigured_WhenUserIdIsEmpty()
        {
            // Arrange
            _currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(_tenantId);
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), _tenantId)).ReturnsAsync(string.Empty);

            var handler = CreateHandler();
            var query = new CheckXIntegrationStatusQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.Configured);
            Assert.Null(result.LastRun);
        }

        [Fact]
        public async Task Handle_ReturnsConfiguredAndLastRun_WhenUserIdIsPresent()
        {
            // Arrange
            var lastRun = DateTime.UtcNow;
            _currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(_tenantId);
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), _tenantId)).ReturnsAsync("user-id");
            _jobService.Setup(x => x.GetLastRunDate($"{_tenantId}_SyncTweets")).Returns(lastRun);

            var handler = CreateHandler();
            var query = new CheckXIntegrationStatusQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.Configured);
            Assert.Equal(lastRun, result.LastRun);
        }
    }
}
