using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Moq;

namespace Churchee.ImageProcessing.Tests.Jobs
{
    public class ImageCropsGeneratorTests
    {
        private readonly Mock<IBlobStore> _blobStoreMock;
        private readonly Mock<IImageProcessor> _imageProcessorMock;
        private readonly ImageCropsGenerator _imageCropsGenerator;

        public ImageCropsGeneratorTests()
        {
            _blobStoreMock = new Mock<IBlobStore>();
            _imageProcessorMock = new Mock<IImageProcessor>();
            _imageCropsGenerator = new ImageCropsGenerator(_blobStoreMock.Object, _imageProcessorMock.Object);
        }

        [Fact]
        public async Task CreateCropsAsync_ShouldCallCreateImageSizeAndCreateImageCrop()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string originalImagePath = "path/to/image.jpg";
            bool overrideExisting = true;

            _imageProcessorMock.Setup(p => p.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<int>(), 0, It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());
            _imageProcessorMock.Setup(p => p.CreateCropAsync(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            var token = CancellationToken.None;

            // Act
            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, originalImagePath, overrideExisting, token);

            // Assert
            _imageProcessorMock.Verify(p => p.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<int>(), 0, It.IsAny<string>(), token), Times.Exactly(7));
            _imageProcessorMock.Verify(p => p.CreateCropAsync(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>(), token), Times.Exactly(7));
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, token), Times.Exactly(14));
        }

        [Fact]
        public async Task CreateImageSize_ShouldCallResizeImageAndSaveAsync()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string fileName = "image";
            string folderPath = "path/to/";
            string extension = ".jpg";
            int width = 200;
            bool overrideExisting = true;

            _imageProcessorMock.Setup(p => p.ResizeImageAsync(It.IsAny<Stream>(), width, 0, extension, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            var token = CancellationToken.None;

            // Act
            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, $"{folderPath}{fileName}{extension}", overrideExisting, token);

            // Assert
            _imageProcessorMock.Verify(p => p.ResizeImageAsync(It.IsAny<Stream>(), width, 0, ".webp", token), Times.AtLeastOnce);
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, token), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateImageCrop_ShouldCallCreateCropAndSaveAsync()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string fileName = "image";
            string folderPath = "path/to/";
            string extension = ".jpg";
            int width = 200;
            bool overrideExisting = true;

            _imageProcessorMock.Setup(p => p.CreateCropAsync(It.IsAny<Stream>(), width, extension, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new MemoryStream());

            // Act
            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, $"{folderPath}{fileName}{extension}", overrideExisting, CancellationToken.None);

            // Assert
            _imageProcessorMock.Verify(p => p.CreateCropAsync(It.IsAny<Stream>(), width, ".webp", It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, default), Times.AtLeastOnce);
        }
    }
}