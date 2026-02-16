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
                TempImagePath = string.Empty, // no image
                ImageFileName = string.Empty
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
                jobServiceMock.Object,
                imageProcessorMock.Object,
                blobStoreMock.Object,
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
            imageProcessorMock.Verify(i => i.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
            blobStoreMock.Verify(b => b.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
            jobServiceMock.Verify(j => j.QueueJob<ImageCropsGenerator>(It.IsAny<Expression<Func<ImageCropsGenerator, Task>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithImage_ProcessImageSavesBlobQueuesJobAndDeletesTempFile()
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
                TempImagePath = tempPath,
                ImageFileName = "photo.JPG",
                ImagePath = "/img/articles",
                ImageAltTag = "alt text"
            };

            var article = new Article(tenantId, Guid.NewGuid(), Guid.Empty, "Old Title", "/old-url", "old desc");

            var repoMock = new Mock<IRepository<Article>>();
            _repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Article>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(article);

            var jobServiceMock = new Mock<IJobService>();

            var imageProcessorMock = new Mock<IImageProcessor>();
            // Return a readable memory stream
            imageProcessorMock
                .Setup(i => i.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream([1, 2, 3]));

            var blobStoreMock = new Mock<IBlobStore>();
            string capturedPath = null!;
            blobStoreMock
                .Setup(b => b.SaveAsync(tenantId, It.IsAny<string>(), It.IsAny<Stream>(), false, It.IsAny<CancellationToken>()))
                .Callback<Guid, string, Stream, bool, CancellationToken>((g, path, s, o, ct) => capturedPath = path)
                .ReturnsAsync("/img/articles/photo.webp");

            var currentUserMock = new Mock<ICurrentUser>();
            currentUserMock.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var sut = new UpdateArticleCommandHandler(
                GetDataStore(),
                jobServiceMock.Object,
                imageProcessorMock.Object,
                blobStoreMock.Object,
                currentUserMock.Object);

            // Act
            var result = await sut.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.IsSuccess.Should().BeTrue();

            // Blob saved and path captured
            blobStoreMock.Verify(b => b.SaveAsync(tenantId, It.IsAny<string>(), It.IsAny<Stream>(), false, It.IsAny<CancellationToken>()), Times.Once);
            capturedPath.Should().NotBeNull();
            capturedPath.Should().NotBeEmpty();
            // Article image set to saved path without .webp
            article.ImageUrl.Should().Be("/img/articles/photo");
            article.ImageAltTag.Should().Be(request.ImageAltTag);

            // Job queued to generate crops
            jobServiceMock.Verify(j => j.QueueJob<ImageCropsGenerator>(It.IsAny<Expression<Func<ImageCropsGenerator, Task>>>()), Times.Once);

            // Temp file deleted
            File.Exists(tempPath).Should().BeFalse();
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