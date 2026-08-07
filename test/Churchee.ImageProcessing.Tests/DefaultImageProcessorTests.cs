using Churchee.Common.Storage;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Churchee.ImageProcessing.Tests
{
    public class DefaultImageProcessorTests
    {

        private DefaultImageProcessor GetDefaultImageProcessor(Mock<IBlobStore>? mockBlobStore = null, Mock<ILogger<DefaultImageProcessor>>? mockLogger = null)
        {
            var logger = mockLogger ?? new Mock<ILogger<DefaultImageProcessor>>();
            var blobStore = mockBlobStore ?? new Mock<IBlobStore>();

            return new DefaultImageProcessor(blobStore.Object, logger.Object);
        }

        [Fact]
        public async Task CreateCrop_ShouldReturnCroppedImageStream()
        {
            // Arrange
            var processor = GetDefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = await processor.CreateCropAsync(stream, 50, ".png", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(50);
            resultImage.Height.Should().Be(100);
        }

        [Fact]
        public async Task ResizeImage_ShouldReturnResizedImageStream()
        {
            // Arrange
            var processor = GetDefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = await processor.ResizeImageAsync(stream, 50, 0, ".png", CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(50);
            resultImage.Height.Should().Be(50);
        }

        [Fact]
        public async Task Process_ShouldReturnOriginalStream_WhenWidthAndHeightAreZero()
        {
            // Arrange
            var processor = GetDefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = await processor.ResizeImageAsync(stream, 0, 0, ".png", CancellationToken.None);
            result.Position = 0;

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(100);
            resultImage.Height.Should().Be(100);
        }

        [Fact]
        public async Task ConvertTempImageToFullImage_ShouldReturnImagePathAndDeleteTempFile()
        {
            // Arrange
            string fileName = "My File.PNG";
            string folderName = "images";
            Guid tenantId = Guid.NewGuid();

            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            // Create a small PNG temp file
            using (var img = new Image<Rgba32>(10, 10))
            {
                await using var fs = File.OpenWrite(tempPath);
                img.SaveAsPng(fs);
            }

            string expectedImagePath = Path.Combine(folderName, $"{Path.GetFileNameWithoutExtension(fileName).ToDevName()}.webp");

            var mockBlobStore = new Mock<IBlobStore>();
            mockBlobStore
                .Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .Returns<Guid, string, Stream, bool, CancellationToken>((id, path, stream, ov, token) => Task.FromResult(path));

            var mockLogger = new Mock<ILogger<DefaultImageProcessor>>();

            var processor = GetDefaultImageProcessor(mockBlobStore, mockLogger);

            // Act
            var result = await processor.ConvertTempImageToFullImage(tempPath, fileName, folderName, tenantId, CancellationToken.None);

            // Assert
            result.Should().Be(expectedImagePath);

            mockBlobStore.Verify(x => x.SaveAsync(
                tenantId,
                It.Is<string>(p => p == expectedImagePath),
                It.IsAny<Stream>(),
                false,
                It.IsAny<CancellationToken>()), Times.Once);

            File.Exists(tempPath).Should().BeFalse();
        }

        [Fact]
        public async Task ConvertTempImageToFullImage_ShouldReturnEmpty_WhenParametersMissing()
        {
            // Arrange
            var mockBlobStore = new Mock<IBlobStore>();
            var mockLogger = new Mock<ILogger<DefaultImageProcessor>>();
            var processor = GetDefaultImageProcessor(mockBlobStore, mockLogger);

            // Act
            var result1 = await processor.ConvertTempImageToFullImage(string.Empty, "file.png", "images", Guid.NewGuid(), CancellationToken.None);
            var result2 = await processor.ConvertTempImageToFullImage("path", string.Empty, "images", Guid.NewGuid(), CancellationToken.None);
            var result3 = await processor.ConvertTempImageToFullImage("path", "file.png", string.Empty, Guid.NewGuid(), CancellationToken.None);

            // Assert
            result1.Should().BeEmpty();
            result2.Should().BeEmpty();
            result3.Should().BeEmpty();

            mockBlobStore.Verify(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ConvertTempImageToFullImage_ShouldLogErrorAndReturnEmpty_WhenBlobSaveThrows()
        {
            // Arrange
            string fileName = "Test.PNG";
            string folderName = "images";
            Guid tenantId = Guid.NewGuid();

            string tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");

            // Create a small PNG temp file
            using (var img = new Image<Rgba32>(10, 10))
            {
                await using var fs = File.OpenWrite(tempPath);
                img.SaveAsPng(fs);
            }

            var mockBlobStore = new Mock<IBlobStore>();
            mockBlobStore
                .Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("boom"));

            var mockLogger = new Mock<ILogger<DefaultImageProcessor>>();

            var processor = GetDefaultImageProcessor(mockBlobStore, mockLogger);

            // Act
            var result = await processor.ConvertTempImageToFullImage(tempPath, fileName, folderName, tenantId, CancellationToken.None);

            // Assert
            result.Should().BeEmpty();

            // Verify an error was logged
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce);

            // Temp file should be deleted even after exception
            File.Exists(tempPath).Should().BeFalse();
        }
    }
}