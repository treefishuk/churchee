using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Features.Queries;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Google.Reviews.Tests.Features.Queries
{
    public class GetOAuthRedirectQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_Correct_Redirect_Url()
        {
            // Arrange
            var mockSettingStore = new Mock<ISettingStore>();
            var mockCurrentUser = new Mock<ICurrentUser>();

            var handler = new GetOAuthRedirectQueryHandler(mockSettingStore.Object, mockCurrentUser.Object);

            var query = new GetOAuthRedirectQuery("example.com", "test-client-id", string.Empty, string.Empty);

            // Act
            string redirectUrl = await handler.Handle(query, CancellationToken.None);

            // Assert
            redirectUrl.Should().Contain("redirect_uri=example.com%2Fmanagement%2Fintegrations%2Fgoogle-reviews%2Fauth");
            redirectUrl.Should().Contain("response_type=code");
            redirectUrl.Should().Contain("scope=https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fbusiness.manage");
        }

    }
}
