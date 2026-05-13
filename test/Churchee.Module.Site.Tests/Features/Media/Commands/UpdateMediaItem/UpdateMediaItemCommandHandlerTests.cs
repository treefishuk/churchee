using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Churchee.Test.Helpers.Validation;
using Hangfire.Common;
using Hangfire.States;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.UpdateMediaItem
{
    public class UpdateMediaItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsNotFound_WhenEntityMissing()
        {
            // Arrange
            var blobStoreMock = new Mock<IBlobStore>();
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();
            var imageProcessorMock = new Mock<IImageProcessor>();
            var mediaItemRepoMock = new Mock<IRepository<MediaItem>>();

            // No items returned from ApplySpecification => not found
            mediaItemRepoMock
                .Setup(x => x.ApplySpecification(It.IsAny<ISpecification<MediaItem>>(), It.IsAny<bool>()))
                .Returns(Enumerable.Empty<MediaItem>().AsQueryable());

            dataStoreMock.Setup(x => x.GetRepository<MediaItem>()).Returns(mediaItemRepoMock.Object);

            var handler = new UpdateMediaItemCommandHandler(
                blobStoreMock.Object,
                dataStoreMock.Object,
                currentUserMock.Object,
                backgroundJobClientMock.Object,
                imageProcessorMock.Object
            );

            var command = new UpdateMediaItemCommand.Builder()
                .SetId(Guid.NewGuid())
                .SetName("Name")
                .SetDescription("Desc")
                .SetOrder(0)
                .Build();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors[0].Property.Should().Be("Id");
            result.Errors[0].Description.Should().Be("Not Found");
        }

        [Fact]
        public async Task Handle_ReturnsWithoutIssue_WhenBase64Missing()
        {
            // Arrange
            var blobStoreMock = new Mock<IBlobStore>();
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();
            var imageProcessorMock = new Mock<IImageProcessor>();

            var tenantId = Guid.NewGuid();
            var folderId = Guid.NewGuid();

            var mediaItem = new MediaItem(tenantId, "old", "/old", "desc", folderId);

            var mediaItemRepoMock = new Mock<IRepository<MediaItem>>();
            mediaItemRepoMock
                .Setup(x => x.ApplySpecification(It.IsAny<ISpecification<MediaItem>>(), It.IsAny<bool>()))
                .Returns(new[] { mediaItem }.AsQueryable());

            dataStoreMock.Setup(x => x.GetRepository<MediaItem>()).Returns(mediaItemRepoMock.Object);

            var handler = new UpdateMediaItemCommandHandler(
                blobStoreMock.Object,
                dataStoreMock.Object,
                currentUserMock.Object,
                backgroundJobClientMock.Object,
                imageProcessorMock.Object
            );

            // Build command without Base64Content -> should trigger Content not found
            var command = new UpdateMediaItemCommand.Builder()
                .SetId(mediaItem.Id)
                .SetName("New Name")
                .SetDescription("New Desc")
                .SetOrder(1)
                .Build();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_SavesImageAndEnqueuesCrops_WhenImageFormat()
        {
            // Arrange
            var blobStoreMock = new Mock<IBlobStore>();
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();
            var imageProcessorMock = new Mock<IImageProcessor>();

            var tenantId = Guid.NewGuid();
            var folderId = Guid.NewGuid();
            string folderPath = "media-folder/";
            string fileName = "My File";
            string fileExtension = ".png";
            string finalFilePath = "/media-folder/myFile.webp";

            var mediaItem = new MediaItem(tenantId, "old", "/old", "desc", folderId) { };

            var mediaItemRepoMock = new Mock<IRepository<MediaItem>>();
            mediaItemRepoMock
                .Setup(x => x.ApplySpecification(It.IsAny<ISpecification<MediaItem>>(), It.IsAny<bool>()))
                .Returns(new[] { mediaItem }.AsQueryable());

            var mediaFolderRepoMock = new Mock<IRepository<MediaFolder>>();
            mediaFolderRepoMock
                .Setup(x => x.GetQueryable())
                .Returns(new[] { new MediaFolder(tenantId, "media-folder", string.Empty) { Path = folderPath } }.AsQueryable());

            dataStoreMock.Setup(x => x.GetRepository<MediaItem>()).Returns(mediaItemRepoMock.Object);
            dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(mediaFolderRepoMock.Object);
            dataStoreMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var webpStream = new MemoryStream(new byte[] { 1, 2, 3 });
            imageProcessorMock.Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(webpStream);

            blobStoreMock
                .Setup(x => x.SaveAsync(tenantId, It.IsAny<string>(), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(finalFilePath);

            var handler = new UpdateMediaItemCommandHandler(
                blobStoreMock.Object,
                dataStoreMock.Object,
                currentUserMock.Object,
                backgroundJobClientMock.Object,
                imageProcessorMock.Object
            );

            var base64 = "data:image/png;base64," + Convert.ToBase64String(new byte[] { 1, 2, 3 });

            var command = new UpdateMediaItemCommand.Builder()
                .SetId(mediaItem.Id)
                .SetName("New Name")
                .SetFileName(fileName)
                .SetExtention(fileExtension)
                .SetBase64Image(base64)
                .SetDescription("New Desc")
                .SetOrder(2)
                .Build();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // Media url should be updated on the entity instance
            mediaItem.MediaUrl.Should().Be(finalFilePath);

            dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));

            // Verify blob saved (path is constructed - verify call occurred)
            blobStoreMock.Verify(x => x.SaveAsync(tenantId, It.Is<string>(p => p.EndsWith(".webp", StringComparison.OrdinalIgnoreCase)), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);

            // Verify background job enqueued for creating crops
            backgroundJobClientMock.Verify(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Once);

        }

        [Fact]
        public async Task Handle_SavesOtherFormats_WhenNotImage()
        {
            // Arrange
            var blobStoreMock = new Mock<IBlobStore>();
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();
            var imageProcessorMock = new Mock<IImageProcessor>();

            var tenantId = Guid.NewGuid();
            var folderId = Guid.NewGuid();
            string folderPath = "docs/";
            string fileName = "Doc File";
            string fileExtension = ".pdf";
            string finalFilePath = "/docs/docFile.pdf";

            var mediaItem = new MediaItem(tenantId, "old", "/old", "desc", folderId) { };

            var mediaItemRepoMock = new Mock<IRepository<MediaItem>>();
            mediaItemRepoMock
                .Setup(x => x.ApplySpecification(It.IsAny<ISpecification<MediaItem>>(), It.IsAny<bool>()))
                .Returns(new[] { mediaItem }.AsQueryable());

            var mediaFolderRepoMock = new Mock<IRepository<MediaFolder>>();
            mediaFolderRepoMock
                .Setup(x => x.GetQueryable())
                .Returns(new[] { new MediaFolder(tenantId, "docs", string.Empty) { Path = folderPath } }.AsQueryable());

            dataStoreMock.Setup(x => x.GetRepository<MediaItem>()).Returns(mediaItemRepoMock.Object);
            dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(mediaFolderRepoMock.Object);
            dataStoreMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);

            blobStoreMock
                .Setup(x => x.SaveAsync(tenantId, It.IsAny<string>(), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()))
                .ReturnsAsync(finalFilePath);

            var handler = new UpdateMediaItemCommandHandler(
                blobStoreMock.Object,
                dataStoreMock.Object,
                currentUserMock.Object,
                backgroundJobClientMock.Object,
                imageProcessorMock.Object
            );

            var base64 = "data:application/pdf;base64," + Convert.ToBase64String(new byte[] { 1, 2, 3 });

            var command = new UpdateMediaItemCommand.Builder()
                .SetId(mediaItem.Id)
                .SetName("Doc Name")
                .SetFileName(fileName)
                .SetExtention(fileExtension)
                .SetBase64Image(base64)
                .SetDescription("Doc Desc")
                .SetOrder(3)
                .Build();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            mediaItem.MediaUrl.Should().Be(finalFilePath);

            dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

            // For non-image, ImageCropsGenerator should NOT be enqueued
            backgroundJobClientMock.Verify(b => b.Create(It.IsAny<Job>(), It.IsAny<IState>()), Times.Never);

            blobStoreMock.Verify(x => x.SaveAsync(tenantId, It.Is<string>(p => p.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}