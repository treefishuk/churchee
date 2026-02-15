using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class CreateArticleModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new CreateArticleModel();

            // Assert
            Assert.Equal(string.Empty, model.Title);
            Assert.Equal(string.Empty, model.Description);
            Assert.Equal(string.Empty, model.Description);
            Assert.Equal(string.Empty, model.Description);
            Assert.Equal("<p></p>", model.Content);
            Assert.NotNull(model.Image);
            Assert.Equal(".jpg,.jpeg,.png,.gif", model.Image.SupportedFileTypes);
            Assert.Equal(string.Empty, model.Image.TempFilePath);
            Assert.Equal(string.Empty, model.Image.ThumbnailUrl);
            Assert.Equal("articles/", model.Image.Path);
            Assert.Null(model.Parent);
            Assert.Null(model.PublishOnDate);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new CreateArticleModel
            {
                Title = null,
                Description = null,
                Content = null,
                Parent = null
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Title)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Content)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Parent)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new CreateArticleModel
            {
                Title = "Test Title",
                Description = "Test Description",
                Content = "<p>Test Content</p>",
                Parent = new DropdownInput { Title = "Parent", Value = "1" }
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }

        [Fact]
        public void Description_ExceedingMaxLength_FailsValidation()
        {
            // Arrange
            var model = new CreateArticleModel
            {
                Title = "Test Title",
                Description = new string('a', 201), // Exceeds MaxLength(200)
                Content = "<p>Test Content</p>",
                Parent = new DropdownInput { Title = "Parent", Value = "1" }
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
        }
    }
}