using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Blog.Commands.UpdateArticle
{
    public class UpdateArticleCommandHandlerTests
    {
        private readonly Mock<IRepository<Article>> _repoMock;

        public UpdateArticleCommandHandlerTests()
        {
            _repoMock = new Mock<IRepository<Article>>();
        }

        [Fact]
        public async Task Handle_Sets_Properites_Correctly()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            var request = new UpdateArticleCommand
            {
                Id = Guid.NewGuid(),
                Title = "New Title",
                Description = "New Desc",
                Content = "<p>content</p>",
                PublishOnDate = DateTime.UtcNow,
                ParentPageId = Guid.NewGuid(),
            };

            var article = new Article(tenantId, Guid.NewGuid(), Guid.Empty, "Old Title", "/old-url", "old desc");

            _repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Article>>(), It.IsAny<CancellationToken>())).ReturnsAsync(article);

            var jobServiceMock = new Mock<IJobService>();
            var imageProcessorMock = new Mock<IImageProcessor>();
            var blobStoreMock = new Mock<IBlobStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var sut = new UpdateArticleCommandHandler(
                GetDataStore(),
                currentUserMock.Object);

            // Act
            var result = await sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Entity updated
            article.Title.Should().Be(request.Title);
            article.Description.Should().Be(request.Description);
            article.Content.Should().Be(request.Content);
            article.LastPublishedDate.Should().Be(request.PublishOnDate);
        }

        private IDataStore GetDataStore()
        {
            var tenantId = Guid.NewGuid();
            var mockArticleRepository = new Mock<IRepository<Article>>();
            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Article>()).Returns(_repoMock.Object);
            return mockDataStore.Object;
        }
    }
}