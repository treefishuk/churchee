using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Features.Commands;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Moq;

namespace Churchee.Module.Google.Reviews.Tests.Features.Commands
{
    public class DisableGoogleReviewSyncCommandHandlerTests
    {

        [Fact]
        public async Task Handle_DeletesTokens()
        {
            // Arrange
            var dataStore = new Mock<IDataStore>();
            var repo = new Mock<IRepository<Token>>();
            var currentUser = new Mock<ICurrentUser>();
            var applicationTenantId = Guid.NewGuid();
            currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(applicationTenantId);

            var accessToken = new Token(Guid.NewGuid(), SettingKeys.GoogleReviewsAccessTokenKey.ToString(), "Value");

            var refreshToken = new Token(Guid.NewGuid(), SettingKeys.GoogleReviewsRefreshTokenKey.ToString(), "Value");

            repo.Setup(x => x.FirstOrDefaultAsync(It.Is<GetTokenByKeySpecification>(s => s.Key == SettingKeys.GoogleReviewsAccessTokenKey.ToString()), It.IsAny<CancellationToken>())).ReturnsAsync(accessToken);

            repo.Setup(x => x.FirstOrDefaultAsync(It.Is<GetTokenByKeySpecification>(s => s.Key == SettingKeys.GoogleReviewsRefreshTokenKey.ToString()), It.IsAny<CancellationToken>())).ReturnsAsync(refreshToken);

            dataStore.Setup(x => x.GetRepository<Token>()).Returns(repo.Object);

            var handler = new DisableGoogleReviewSyncCommandHandler(dataStore.Object, currentUser.Object);

            // Act
            var result = await handler.Handle(new DisableGoogleReviewSyncCommand(), CancellationToken.None);

            // Assert
            repo.Verify(x => x.PermanentDelete(accessToken), Times.Once);
            repo.Verify(x => x.PermanentDelete(refreshToken), Times.Once);
            dataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
