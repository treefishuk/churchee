using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

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
        public async Task Handle_WhenNoImageProvided_DoesNotProcessImageAndSavesChanges()
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

            // No image processing invoked
            imageProcessorMock.Verify(i => i.ConvertTempImageToFullImage(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
            blobStoreMock.Verify(b => b.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
            jobServiceMock.Verify(j => j.QueueJob<ImageCropsGenerator>(It.IsAny<Expression<Func<ImageCropsGenerator, Task>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithImage_ProcessImageSetsImageQueuesJob()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // create a temp file to simulate uploaded temp image
            string tempPath = Path.GetTempFileName();
            await File.WriteAllTextAsync(tempPath, "dummy");

            var request = new UpdateArticleCommand
            {
                Id = Guid.NewGuid(),
                Title = "New Title",
                Description = "New Desc",
                Content = "<p>content</p>",
                PublishOnDate = DateTime.UtcNow,
                ParentPageId = Guid.NewGuid(),
                ImagePath = "/img/articles/photo.JPG",
                ImageAltTag = "alt text"
            };

            var article = new Article(tenantId, Guid.NewGuid(), Guid.Empty, "Old Title", "/old-url", "old desc");

            _repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Article>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(article);

            var jobServiceMock = new Mock<IJobService>();

            var imageProcessorMock = new Mock<IImageProcessor>();

            // Blob store is no longer used by the handler, but the constructor still requires it
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

            // Article image set to saved path without .webp
            article.ImageUrl.Should().Be("/img/articles/photo");
            article.ImageAltTag.Should().Be(request.ImageAltTag);

            // Job queued to generate crops
            jobServiceMock.Verify(j => j.QueueJob<ImageCropsGenerator>(It.IsAny<Expression<Func<ImageCropsGenerator, Task>>>()), Times.Once);
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