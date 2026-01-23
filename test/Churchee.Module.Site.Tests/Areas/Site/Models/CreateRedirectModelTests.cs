using Churchee.Module.Site.Areas.Site.Models;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Site.Tests.Areas.Site.Models
{
    public class CreateRedirectModelTests
    {
        [Fact]
        public void Constructor_InitializesPropertiesWithDefaults()
        {
            // Act
            var model = new CreateRedirectModel();

            // Assert
            Assert.Equal(string.Empty, model.Path);
            Assert.Equal("Select a Page", model.Page.Title);
        }

        [Fact]
        public void Model_WithMissingRequiredFields_FailsValidation()
        {
            // Arrange
            var model = new CreateRedirectModel
            {
                Path = null,
                Page = null
            };

            // Act
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, null, null);
            var isValid = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Page)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(model.Path)));
        }
    }
}