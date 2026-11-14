using Churchee.Module.Site.Areas.Site.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class StylesInputModelTests
    {
        [Fact]
        public void DefaultConstructor_InitializesStylesWithEmptyString()
        {
            // Act
            var model = new StylesInputModel();

            // Assert
            Assert.Equal(string.Empty, model.Styles);
        }

        [Fact]
        public void ParameterizedConstructor_SetsStyles()
        {
            // Arrange
            string css = "body { color: red; }";

            // Act
            var model = new StylesInputModel(css);

            // Assert
            Assert.Equal(css, model.Styles);
        }

        [Fact]
        public void Model_WithoutValidationAttributes_AlwaysValid()
        {
            // Arrange
            var model = new StylesInputModel("body { background: blue; }");

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            bool isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(results);
        }
    }
}