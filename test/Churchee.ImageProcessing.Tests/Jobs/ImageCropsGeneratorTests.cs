using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

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
        public async Task CreateCropsAsync_ShouldCreateAllResizeAndCropVariants()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string originalImagePath = "path/to/image.jpg";
            bool overrideExisting = true;
            var token = CancellationToken.None;

            _blobStoreMock.Setup(b => b.GetReadStreamAsync(applicationTenantId, originalImagePath, token))
                .ReturnsAsync(CreateImageStream());

            // Act
            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, originalImagePath, overrideExisting, token);

            // Assert
            _imageProcessorMock.Verify(p => p.ResizeImageAsync(It.IsAny<Stream>(), It.IsAny<int>(), 0, It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _imageProcessorMock.Verify(p => p.CreateCropAsync(It.IsAny<Stream>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, token), Times.Exactly(14));
        }

        [Fact]
        public async Task CreateCropsAsync_ShouldGenerateWebpOutputsForEachConfiguredVariant()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            string fileName = "image";
            string folderPath = "path/to/";
            string extension = ".jpg";
            bool overrideExisting = true;
            var token = CancellationToken.None;

            _blobStoreMock.Setup(b => b.GetReadStreamAsync(applicationTenantId, $"{folderPath}{fileName}{extension}", token))
                .ReturnsAsync(CreateImageStream());

            // Act
            await _imageCropsGenerator.CreateCropsAsync(applicationTenantId, $"{folderPath}{fileName}{extension}", overrideExisting, token);

            // Assert
            _blobStoreMock.Verify(b => b.SaveAsync(
                applicationTenantId,
                It.Is<string>(path => path.Contains("_t.webp") || path.Contains("_ct.webp") || path.Contains("_hd.webp") || path.Contains("_chd.webp")),
                It.IsAny<Stream>(),
                overrideExisting,
                token), Times.AtLeast(1));
            _blobStoreMock.Verify(b => b.SaveAsync(applicationTenantId, It.IsAny<string>(), It.IsAny<Stream>(), overrideExisting, token), Times.Exactly(14));
        }

        private static MemoryStream CreateImageStream()
        {
            var image = new Image<Rgba32>(1200, 800);
            var stream = new MemoryStream();
            image.SaveAsJpeg(stream);
            stream.Position = 0;
            return stream;
        }
    }
}