using Churchee.Common.ValueTypes;
using Churchee.Module.Site.Areas.Site.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class CreateMediaItemModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new CreateMediaItemModel();

            // Assert
            Assert.Equal(string.Empty, model.Name);
            Assert.Equal(string.Empty, model.Description);
            Assert.Equal(string.Empty, model.AdditionalContent);
            Assert.Equal(string.Empty, model.LinkUrl);
            Assert.Equal(string.Empty, model.CssClass);
            Assert.NotNull(model.File);
            Assert.Equal(10, model.Order);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new CreateMediaItemModel
            {
                Name = null,
                Description = null,
                File = null,
                Order = 0
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Name)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.File)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new CreateMediaItemModel
            {
                Name = "Test Name",
                Description = "Test Description",
                File = new Upload(),
                Order = 1,
                LinkUrl = "https://example.com",
                CssClass = "testclass",
                AdditionalContent = "<p>Extra</p>"
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void CssClass_WithInvalidFormat_FailsValidation()
        {
            // Arrange
            var model = new CreateMediaItemModel
            {
                Name = "Test Name",
                Description = "Test Description",
                File = new Upload(),
                Order = 1,
                CssClass = "Invalid Class" // Not a single lowercase word
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.CssClass)));
        }
    }
}