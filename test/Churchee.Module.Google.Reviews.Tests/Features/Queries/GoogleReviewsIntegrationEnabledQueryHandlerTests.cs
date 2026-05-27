using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Features.Queries;
using Churchee.Module.Google.Reviews.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Google.Reviews.Tests.Features.Queries
{
    public class GoogleReviewsIntegrationEnabledQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_True_When_Integration_Is_Enabled()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Token>>();
            mockRepo.Setup(r => r.AnyAsync(It.IsAny<HasAccessTokenSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            var dataStoreMock = new Mock<IDataStore>();
            dataStoreMock.Setup(ds => ds.GetRepository<Token>()).Returns(mockRepo.Object);
            var handler = new GoogleReviewsIntegrationEnabledQueryHandler(dataStoreMock.Object);

            // Act
            bool result = await handler.Handle(new GoogleReviewsIntegrationEnabledQuery(), CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

    }
}
