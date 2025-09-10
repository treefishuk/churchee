using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.CreateMediaItem
{
    public class CreateMediaItemCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesMediaItemAndSavesChanges()
        {
            // Arrange
            var blobStoreMock = new Mock<IBlobStore>();
            var dataStoreMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var backgroundJobClientMock = new Mock<Hangfire.IBackgroundJobClient>();
            var imageProcessorMock = new Mock<IImageProcessor>();
            var mediaFolderRepoMock = new Mock<IRepository<MediaFolder>>();
            var mediaItemRepoMock = new Mock<IRepository<MediaItem>>();
            var tenantId = Guid.NewGuid();
            var folderId = Guid.NewGuid();
            string folderPath = "folder-path";
            string fileName = "file";
            string filePath = $"/{folderPath}{fileName}.webp";
            string base64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUA";
            var webpStream = new MemoryStream(new byte[] { 1, 2, 3 });
            string finalFilePath = "/folder-pathfile.webp";

            currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);
            dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(mediaFolderRepoMock.Object);
            dataStoreMock.Setup(x => x.GetRepository<MediaItem>()).Returns(mediaItemRepoMock.Object);
            mediaFolderRepoMock.Setup(x => x.GetQueryable()).Returns(new[] { new MediaFolder(tenantId, "folder", string.Empty) { Path = folderPath } }.AsQueryable());
            imageProcessorMock.Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(webpStream);
            blobStoreMock.Setup(x => x.SaveAsync(tenantId, It.IsAny<string>(), webpStream, true, It.IsAny<CancellationToken>())).ReturnsAsync(finalFilePath);

            var handler = new CreateMediaItemCommandHandler(blobStoreMock.Object, dataStoreMock.Object, currentUserMock.Object, backgroundJobClientMock.Object, imageProcessorMock.Object);
            var command = new CreateMediaItemCommand
            {
                Name = "name",
                FileName = fileName,
                FileExtension = ".webp",
                Base64Content = base64,
                FolderId = folderId
            };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mediaItemRepoMock.Verify(x => x.Create(It.IsAny<MediaItem>()), Times.Once);
            dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
