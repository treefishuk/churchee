using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Features.Commands;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers.Validation;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Moq;

namespace Churchee.Module.Google.Reviews.Tests.Features.Commands
{
    public class EnableGoogleReviewsIntegrationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_Successful_CommandResponse()
        {
            // Arrange
            var settingStore = new Mock<ISettingStore>();
            var currentUser = new Mock<ICurrentUser>();
            var dataStore = new Mock<IDataStore>();

            var mockTokenRepo = new Mock<IRepository<Token>>();

            dataStore.Setup(s => s.GetRepository<Token>()).Returns(mockTokenRepo.Object);

            var jobService = new Mock<IJobService>();
            var applicationTenantId = Guid.NewGuid();
            currentUser.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(applicationTenantId);
            currentUser.Setup(x => x.GetUserId()).Returns("UserId");
            settingStore.Setup(x => x.GetSettingValue(SettingKeys.ClientId, applicationTenantId)).ReturnsAsync("ClientId");
            settingStore.Setup(x => x.GetSettingValue(SettingKeys.ClientSecret, applicationTenantId)).ReturnsAsync("ClientSecret");
            var mockAuthorizationCodeFlow = new Mock<IAuthorizationCodeFlow>();
            mockAuthorizationCodeFlow.Setup(s => s.ExchangeCodeForTokenAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new TokenResponse { AccessToken = "AccessToken", RefreshToken = "Refresh" });

            var handler = new EnableGoogleReviewsIntegrationCommandHandler(settingStore.Object, currentUser.Object, dataStore.Object, jobService.Object, mockAuthorizationCodeFlow.Object);


            var command = new EnableGoogleReviewsIntegrationCommand("AuthCode", "https://example.com");

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
