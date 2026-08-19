using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.Site.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.Features.Pages.Queries.GetPageDetails
{
    public class GetPageDetailsRequestHandlerTests
    {
        [Fact]
        public async Task Handle_Returns_Result_When_Found()
        {
            // Arrange
            var pageId = Guid.NewGuid();

            var response = new GetPageDetailsResponse
            {
                Title = "Test Page",
            };

            var mockRepo = new Mock<IRepository<Page>>();
            mockRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<PageWithContentAndPropertiesSpecification>(), It.IsAny<Expression<Func<Page, GetPageDetailsResponse>>>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

            var mockStore = new Mock<IDataStore>();
            mockStore.Setup(s => s.GetRepository<Page>()).Returns(mockRepo.Object);

            var handler = new GetPageDetailsRequestHandler(mockStore.Object);
            var request = new GetPageDetailsRequest(pageId: pageId);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Handle_ReturnsNull_WhenNoPageFound()
        {
            // Arrange
            var pageId = Guid.NewGuid();

            var mockRepo = new Mock<IRepository<Page>>();

            var mockStore = new Mock<IDataStore>();
            mockStore.Setup(s => s.GetRepository<Page>()).Returns(mockRepo.Object);

            var handler = new GetPageDetailsRequestHandler(mockStore.Object);
            var request = new GetPageDetailsRequest(pageId: pageId);

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeNull();
        }


    }
}