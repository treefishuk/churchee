using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Favicons.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Favicons
{
    public class GenerateFaviconCommandHandlerTests
    {
        private readonly Mock<IBlobStore> _blobStoreMock = new();
        private readonly Mock<IDataStore> _dataStoreMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IImageProcessor> _imageProcessorMock = new();
        private readonly Mock<IRepository<MediaItem>> _mediaRepoMock = new();

        private GenerateFaviconCommandHandler CreateHandler()
        {
            _dataStoreMock.Setup(ds => ds.GetRepository<MediaItem>()).Returns(_mediaRepoMock.Object);
            return new GenerateFaviconCommandHandler(
                _blobStoreMock.Object,
                _dataStoreMock.Object,
                _currentUserMock.Object,
                _imageProcessorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldProcessAndSaveAllFavicons()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var folderId = Guid.NewGuid();
            string base64 = "data:image/png;base64," + Convert.ToBase64String(new byte[] { 1, 2, 3, 4 });
            var command = new GenerateFaviconCommand(base64, folderId);

            _currentUserMock.Setup(cu => cu.GetApplicationTenantId()).ReturnsAsync(tenantId);

            // Setup image processor to return a dummy stream for any resize
            _imageProcessorMock
                .Setup(ip => ip.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => new MemoryStream([5, 6, 7, 8]));

            // Setup blob store to just return the path
            _blobStoreMock
                .Setup(bs => bs.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid id, string path, Stream s, bool o, CancellationToken t) => path);

            // Setup repository and data store
            _mediaRepoMock.Setup(r => r.Create(It.IsAny<MediaItem>()));
            _dataStoreMock.Setup(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = CreateHandler();

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);

            // Verify that all expected images are saved
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/android-chrome-512x512.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/android-chrome-192x192.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/favicon-48x48.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/favicon-32x32.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/favicon-16x16.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/apple-touch-icon.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/favicon.ico", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);
            _blobStoreMock.Verify(bs => bs.SaveAsync(tenantId, "/favicons/apple-touch-icon_s.png", It.IsAny<Stream>(), true, It.IsAny<CancellationToken>()), Times.Once);

            // Verify that media item is created and changes are saved
            _mediaRepoMock.Verify(r => r.Create(It.IsAny<MediaItem>()), Times.Once);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
