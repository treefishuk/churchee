using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Churchee.ImageProcessing.Tests
{
    public class DefaultImageProcessorTests
    {
        [Fact]
        public void CreateCrop_ShouldReturnCroppedImageStream()
        {
            // Arrange
            var processor = new DefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = processor.CreateCrop(stream, 50, ".png");

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(50);
            resultImage.Height.Should().Be(100);
        }

        [Fact]
        public void ResizeImage_ShouldReturnResizedImageStream()
        {
            // Arrange
            var processor = new DefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = processor.ResizeImage(stream, 50, 0, ".png");

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(50);
            resultImage.Height.Should().Be(50);
        }

        [Fact]
        public void Process_ShouldReturnOriginalStream_WhenWidthAndHeightAreZero()
        {
            // Arrange
            var processor = new DefaultImageProcessor();
            var image = new Image<Rgba32>(100, 100);
            var stream = new MemoryStream();
            image.SaveAsPng(stream);
            stream.Position = 0;

            // Act
            var result = processor.ResizeImage(stream, 0, 0, ".png");
            result.Position = 0;

            // Assert
            result.Should().NotBeNull();
            var resultImage = Image.Load(result);
            resultImage.Width.Should().Be(100);
            resultImage.Height.Should().Be(100);
        }
    }
}