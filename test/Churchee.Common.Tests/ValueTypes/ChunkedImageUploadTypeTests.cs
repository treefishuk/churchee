using Churchee.Common.ValueTypes;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Components.Forms;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Common.Tests.ValueTypes
{
    public class ChunkedImageUploadTypeTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesToEmptyStrings()
        {
            // Act
            var model = new ChunkedImageUploadType();

            // Assert
            model.TempFilePath.Should().BeEmpty();
            model.Path.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ReturnsRequiredError_WhenFileIsNull_AndRequiredAttributePresent()
        {
            // Arrange
            var model = new ChunkedImageUploadType
            {
                File = null,
                Description = "desc"
            };
            var validationContext = new ValidationContext(model)
            {
                Items = { [typeof(RequiredAttribute)] = true }
            };

            // Act
            var results = model.Validate(validationContext);

            // Assert
            results.Should().ContainSingle(r => r.ErrorMessage == "Required");
        }

        [Fact]
        public void Validate_ReturnsNoError_WhenFileIsNotNull_AndRequiredAttributePresent()
        {
            // Arrange
            var browserFileMock = new Mock<IBrowserFile>();
            var model = new ChunkedImageUploadType
            {
                File = browserFileMock.Object,
                Description = "desc"
            };
            var validationContext = new ValidationContext(model)
            {
                Items = { [typeof(RequiredAttribute)] = true }
            };

            // Act
            var results = model.Validate(validationContext);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ReturnsNoError_WhenRequiredAttributeNotPresent()
        {
            // Arrange
            var model = new ChunkedImageUploadType
            {
                File = null,
                Description = "desc"
            };
            var validationContext = new ValidationContext(model);

            // Act
            var results = model.Validate(validationContext);

            // Assert
            results.Should().BeEmpty();
        }

        [Fact]
        public void Description_IsRequired()
        {
            // Arrange
            var model = new ChunkedImageUploadType
            {
                Description = null
            };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(r => r.MemberNames.Contains(nameof(ChunkedImageUploadType.Description)));
        }
    }
}