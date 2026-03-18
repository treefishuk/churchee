using Churchee.Module.Google.Reviews.Areas.Integrations.Models;
using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Google.Reviews.Tests.Areas.Integrations.Models
{
    public class InputModelTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializePropertiesToEmptyStrings()
        {
            // Act
            var model = new InputModel();

            // Assert
            model.ClientId.Should().BeEmpty();
            model.ClientSecret.Should().BeEmpty();
            model.BusinessProfileId.Should().BeEmpty();
        }

        [Fact]
        public void Validation_ShouldFail_WhenRequiredPropertiesAreMissingOrEmpty()
        {
            // Arrange
            var model = new InputModel(); // default ctor sets empty strings which should violate [Required]

            // Act
            var validationResults = ValidateModel(model);

            // Assert
            validationResults.Should().NotBeNull();
            validationResults.Count.Should().Be(3);
            validationResults.Any(a => a.ErrorMessage == "The ClientSecret field is required.").Should().BeTrue();
            validationResults.Any(a => a.ErrorMessage == "The BusinessProfileId field is required.").Should().BeTrue();
            validationResults.Any(a => a.ErrorMessage == "The ClientId field is required.").Should().BeTrue();
        }

        private static List<ValidationResult> ValidateModel(object model)
        {
            var ctx = new ValidationContext(model, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(model, ctx, results, validateAllProperties: true);
            return results;
        }
    }
}