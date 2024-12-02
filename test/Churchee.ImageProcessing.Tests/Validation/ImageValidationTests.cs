using Churchee.ImageProcessing.Validation;
using FluentAssertions;

namespace Churchee.ImageProcessing.Tests.Validation
{
    public class ImageValidationTests
    {

        [Fact]
        public void BeAValidImageExtension_ShouldReturnTrue_WhenExtensionIsValid()
        {
            // Arrange
            var extension = ".jpg";

            // Act
            var result = ImageValidation.BeAValidImageExtension(extension);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeAValidImageExtension_ShouldReturnFalse_WhenExtensionIsInvalid()
        {
            // Arrange
            var extension = ".bmp";

            // Act
            var result = ImageValidation.BeAValidImageExtension(extension);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BeValidImage_ShouldReturnTrue_WhenBase64ImageIsValid()
        {
            // Arrange
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";

            // Act
            var result = ImageValidation.BeValidImage(base64Image);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeValidImage_ShouldReturnFalse_WhenBase64ImageIsInValid()
        {
            // Arrange
            string base64Image = "NotABase64Image";

            // Act
            var result = ImageValidation.BeValidImage(base64Image);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BeValidImage_ShouldReturnFalse_WhenStringIsEmpty()
        {
            // Act  
            var result = ImageValidation.BeValidImage(string.Empty);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BeExpectedFormat_ShouldReturnTrue_WhenBase64ImageIsInExpectedFormat()
        {
            // Arrange
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";
            string extension = ".png";

            // Act
            var result = ImageValidation.BeExpectedFormat(base64Image, extension);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BeExpectedFormat_ShouldReturnFalse_WhenBase64ImageIsNotInExpectedFormat()
        {
            // Arrange
            string base64Image = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAAEnQAABJ0Ad5mH3gAAAANSURBVBhXY2BgYPgPAAEEAQBwIGULAAAAAElFTkSuQmCC";
            string extension = ".jpg";

            // Act
            var result = ImageValidation.BeExpectedFormat(base64Image, extension);

            // Assert
            result.Should().BeFalse();
        }
    }
}
