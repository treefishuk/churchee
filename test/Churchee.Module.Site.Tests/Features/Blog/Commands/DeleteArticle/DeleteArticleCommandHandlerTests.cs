using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.DeleteArticle
{

    public class DeleteArticleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeletesArticleAndSavesChanges()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<Article>>();
            var articleId = Guid.NewGuid();
            storageMock.Setup(x => x.GetRepository<Article>()).Returns(repoMock.Object);
            var handler = new DeleteArticleCommandHandler(storageMock.Object);
            var command = new DeleteArticleCommand(articleId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(x => x.SoftDelete(articleId), Times.Once);
            storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
