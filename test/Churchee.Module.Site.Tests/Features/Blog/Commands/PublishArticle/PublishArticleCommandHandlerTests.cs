using Ardalis.Specification;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.PublishArticle
{
    public class PublishArticleCommandHandlerTests
    {

        [Fact]
        public async Task PublishArticleCommandHandler_Handle_Calls_Publish_And_Returns_CommandResponse()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockArticleRepository = new Mock<IRepository<Article>>();
            var mockArticle = new Mock<Article>();
            mockDataStore.Setup(ds => ds.GetRepository<Article>()).Returns(mockArticleRepository.Object);
            mockArticleRepository.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Article>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockArticle.Object);
            var handler = new PublishArticleCommandHandler(mockDataStore.Object);
            var command = new PublishArticleCommand(Guid.NewGuid());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeOfType<CommandResponse>();
            mockArticle.Verify(a => a.Publish(), Times.Once);
            mockDataStore.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
