using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class PageEditModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new PageEditModel();

            // Assert
            Assert.Equal(string.Empty, model.Title);
            Assert.Equal(string.Empty, model.Description);
            Assert.NotNull(model.Parent);
            Assert.Equal(10, model.Order);
            Assert.NotNull(model.ContentItems);
            Assert.Empty(model.ContentItems);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new PageEditModel
            {
                Title = null,
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Title)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new PageEditModel
            {
                Title = "Test Title",
                Description = "Test Description",
                Parent = new DropdownInput { Title = "Parent", Value = "1" },
                Order = 1,
                ContentItems =
                [
                    new GetPageDetailsResponseContentItem
                    {
                        PageTypeContentId = Guid.NewGuid(),
                        Type = "text",
                        Title = "Content Title",
                        DevName = "devName",
                        Value = "Some value"
                    }
                ]
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
        public void Title_ExceedingMaxLength_FailsValidation()
        {
            // Arrange
            var model = new PageEditModel
            {
                Title = new string('a', 101), // Exceeds MaxLength(100)
                Description = "Test Description",
                Parent = new DropdownInput { Title = "Parent", Value = "1" },
                Order = 1
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Title)));
        }

        [Fact]
        public void Description_ExceedingMaxLength_FailsValidation()
        {
            // Arrange
            var model = new PageEditModel
            {
                Title = "Test Title",
                Description = new string('a', 201), // Exceeds MaxLength(200)
                Parent = new DropdownInput { Title = "Parent", Value = "1" },
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