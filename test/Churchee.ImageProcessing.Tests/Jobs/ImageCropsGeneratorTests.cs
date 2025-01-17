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
        public async Task CreateCrops_ShouldCallCreateImageSizeAndCreateImageCrop()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            var originalImagePath = "path/to/image.jpg";
            var streamBytes = new byte[] { 1, 2, 3 };
            var overrideExisting = true;

            _imageProcessorMock.Setup(p => p.ResizeImage(It.IsAny<Stream>(), It.IsAny<int>(), 0, It.IsAny<string>()))
                .Returns(new MemoryStream());
            _imageProcessorMock.Setup(p => p.CreateCrop(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>()))
                .Returns(new MemoryStream());

            // Act
            await _imageCropsGenerator.CreateCrops(applicationTenantId, originalImagePath, streamBytes, overrideExisting);

            // Assert
            _imageProcessorMock.Verify(p => p.ResizeImage(It.IsAny<Stream>(), It.IsAny<int>(), 0, It.IsAny<string>()), Times.Exactly(7));
            _imageProcessorMock.Verify(p => p.CreateCrop(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>()), Times.Exactly(7));
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, default), Times.Exactly(14));
        }

        [Fact]
        public async Task CreateImageSize_ShouldCallResizeImageAndSaveAsync()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            var fileName = "image";
            var folderPath = "path/to/";
            var extension = ".jpg";
            var streamBytes = new byte[] { 1, 2, 3 };
            var width = 200;
            var overrideExisting = true;

            _imageProcessorMock.Setup(p => p.ResizeImage(It.IsAny<Stream>(), width, 0, extension))
                .Returns(new MemoryStream());

            // Act
            await _imageCropsGenerator.CreateCrops(applicationTenantId, $"{folderPath}{fileName}{extension}", streamBytes, overrideExisting);

            // Assert
            _imageProcessorMock.Verify(p => p.ResizeImage(It.IsAny<Stream>(), width, 0, extension), Times.AtLeastOnce);
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, default), Times.AtLeastOnce);
        }

        [Fact]
        public async Task CreateImageCrop_ShouldCallCreateCropAndSaveAsync()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            var fileName = "image";
            var folderPath = "path/to/";
            var extension = ".jpg";
            var streamBytes = new byte[] { 1, 2, 3 };
            var width = 200;
            var overrideExisting = true;

            _imageProcessorMock.Setup(p => p.CreateCrop(It.IsAny<Stream>(), width, extension))
                .Returns(new MemoryStream());

            // Act
            await _imageCropsGenerator.CreateCrops(applicationTenantId, $"{folderPath}{fileName}{extension}", streamBytes, overrideExisting);

            // Assert
            _imageProcessorMock.Verify(p => p.CreateCrop(It.IsAny<Stream>(), width, extension), Times.AtLeastOnce);
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, default), Times.AtLeastOnce);
        }
    }
}