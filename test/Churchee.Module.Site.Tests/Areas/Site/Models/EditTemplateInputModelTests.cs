using Churchee.Module.Site.Areas.Site.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class EditTemplateInputModelTests
    {
        [Fact]
        public void DefaultConstructor_InitializesContentWithEmptyString()
        {
            // Act
            var model = new EditTemplateInputModel();

            // Assert
            Assert.Equal(string.Empty, model.Content);
            Assert.Equal(Guid.Empty, model.Id);
        }

        [Fact]
        public void ParameterizedConstructor_SetsProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var content = "Test Content";

            // Act
            var model = new EditTemplateInputModel(id, content);

            // Assert
            Assert.Equal(id, model.Id);
            Assert.Equal(content, model.Content);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new EditTemplateInputModel
            {
                Id = Guid.Empty,
                Content = null
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Id)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Content)));
        }

        [Fact]
        public void Model_WithValidFields_PassesValidation()
        {
            // Arrange
            var model = new EditTemplateInputModel
            {
                Id = Guid.NewGuid(),
                Content = "Some template content"
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