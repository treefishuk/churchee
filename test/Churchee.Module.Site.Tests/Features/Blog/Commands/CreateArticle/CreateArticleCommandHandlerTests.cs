using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Module.Site.Specifications;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.CreateArticle
{
    public class CreateArticleCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesArticleAndSavesChanges()
        {
            // Arrange
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var pageRepoMock = new Mock<IRepository<Page>>();
            var articleRepoMock = new Mock<IRepository<Article>>();
            var pageTypeRepoMock = new Mock<IRepository<PageType>>();

            var imageProcessorMock = new Mock<IImageProcessor>();
            var jobServiceMock = new Mock<IJobService>();
            var blobStoreMock = new Mock<IBlobStore>();


            var tenantId = Guid.NewGuid();
            var parentPageId = Guid.NewGuid();
            var detailPageTypeId = Guid.NewGuid();
            string parentUrl = "/parent";

            currentUserMock.Setup(x => x.GetApplicationTenantId())
                .ReturnsAsync(tenantId);

            dataStoreMock.Setup(x => x.GetRepository<Page>())
                .Returns(pageRepoMock.Object);
            dataStoreMock.Setup(x => x.GetRepository<Article>())
                .Returns(articleRepoMock.Object);
            dataStoreMock.Setup(x => x.GetRepository<PageType>())
                .Returns(pageTypeRepoMock.Object);

            pageTypeRepoMock.Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<PageTypeFromSystemKeySpecification>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<PageType, Guid>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(detailPageTypeId);

            pageRepoMock.Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<PageFromIdSpecification>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Page, string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(parentUrl);

            var handler = new CreateArticleCommandHandler(dataStoreMock.Object, currentUserMock.Object, jobServiceMock.Object, imageProcessorMock.Object, blobStoreMock.Object);
            var command = new CreateArticleCommand
            {
                Title = "Test Article",
                Description = "Test Description",
                Content = "Test Content",
                PublishOnDate = DateTime.UtcNow,
                ParentPageId = parentPageId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            articleRepoMock.Verify(x => x.Create(It.IsAny<Article>()), Times.Once);
            dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
