using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class EditArticleModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new EditArticleModel();

            // Assert
            Assert.Equal(string.Empty, model.Title);
            Assert.Equal(string.Empty, model.Description);
            Assert.Equal("<p></p>", model.Content);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new EditArticleModel
            {
                Title = null,
                Description = null,
                Content = null
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Title)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Content)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new EditArticleModel
            {
                Title = "Thing",
                Description = "Description",
                Content = "<p></p>",
                Parent = new DropdownInput { Value = Guid.NewGuid().ToString() }
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }

}
