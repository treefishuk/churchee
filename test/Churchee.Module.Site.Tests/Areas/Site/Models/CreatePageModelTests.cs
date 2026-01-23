using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class CreatePageModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new CreatePageModel();

            // Assert
            Assert.Equal(string.Empty, model.Title);
            Assert.Equal(string.Empty, model.Description);
            Assert.NotNull(model.PageType);
            Assert.NotNull(model.Parent);
            Assert.Equal(10, model.Order);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new CreatePageModel
            {
                Title = null,
                Description = null,
                PageType = null,
                Parent = null,
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Title)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.PageType)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Parent)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new CreatePageModel
            {
                Title = "Test Title",
                Description = "Test Description",
                PageType = new DropdownInput { Title = "Type", Value = "1" },
                Parent = new DropdownInput { Title = "Parent", Value = "2" },
                Order = 1
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
        public void Description_ExceedingMaxLength_FailsValidation()
        {
            // Arrange
            var model = new CreatePageModel
            {
                Title = "Test Title",
                Description = new string('a', 201), // Exceeds MaxLength(200)
                PageType = new DropdownInput { Title = "Type", Value = "1" },
                Parent = new DropdownInput { Title = "Parent", Value = "2" },
                Order = 1
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Description)));
        }
    }
}