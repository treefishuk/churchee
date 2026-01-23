using Churchee.Common.ValueTypes;
using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.Tests.ValueTypes
{
    public class UploadTests
    {
        [Fact]
        public void Validate_ShouldReturnValidationError_WhenValueIsRequiredAndEmpty()
        {
            // Arrange
            var upload = new Upload();
            var validationContext = new ValidationContext(upload, null, null);
            validationContext.MemberName = nameof(upload.Value);
            validationContext.DisplayName = nameof(upload.Value);
            validationContext.Items.Add(typeof(RequiredAttribute), new RequiredAttribute());

            // Act
            var results = upload.Validate(validationContext);

            // Assert
            results.Should().ContainSingle(result => result.ErrorMessage == "Required");
        }

        [Fact]
        public void Validate_ShouldNotReturnValidationError_WhenValueIsNotRequired()
        {
            // Arrange
            var upload = new Upload();
            var validationContext = new ValidationContext(upload, null, null);

            // Act
            var results = upload.Validate(validationContext);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldNotReturnValidationError_WhenValueIsRequiredAndNotEmpty()
        {
            // Arrange
            var upload = new Upload { Value = "some value" };
            var validationContext = new ValidationContext(upload, null, null);
            validationContext.MemberName = nameof(upload.Value);
            validationContext.DisplayName = nameof(upload.Value);
            validationContext.Items.Add(typeof(RequiredAttribute), new RequiredAttribute());

            // Act
            var results = upload.Validate(validationContext);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Path_Property_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var upload = new Upload();
            var expectedPath = "/uploads/file.txt";

            // Act
            upload.Path = expectedPath;
            var actualPath = upload.Path;

            // Assert
            Assert.Equal(expectedPath, actualPath);
        }

        [Fact]
        public void SupportedFileTypes_Property_SetAndGet_WorksCorrectly()
        {
            // Arrange
            var upload = new Upload();
            var expectedTypes = new[] { ".jpg", ".png", ".pdf" };

            // Act
            upload.SupportedFileTypes = expectedTypes.ToString();
            var actualTypes = upload.SupportedFileTypes;

            // Assert
            Assert.Equal(expectedTypes.ToString(), actualTypes);
        }

    }

}