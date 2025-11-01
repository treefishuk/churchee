using Churchee.Module.Site.Areas.Site.Models;
using Churchee.Module.UI.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class PageTypeContentItemModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new PageTypeContentItemModel();

            // Assert
            Assert.NotEqual(Guid.Empty, model.Id);
            Assert.Equal("New Content Type", model.Name);
            Assert.NotNull(model.Type);
            Assert.Equal(0, model.Order);
            Assert.False(model.Required);
            Assert.NotNull(model.Type.Data);
            Assert.Equal(model.Type.Title, model.Type.Value);
        }

        [Fact]
        public void Model_WithEmptyGuid_FailsValidation()
        {
            // Arrange
            var model = new PageTypeContentItemModel
            {
                Id = Guid.Empty
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Id)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new PageTypeContentItemModel
            {
                Id = Guid.NewGuid(),
                Name = "Content Name",
                Type = new DropdownInput { Title = "SimpleText", Value = "SimpleText" },
                Required = true,
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
    }
}