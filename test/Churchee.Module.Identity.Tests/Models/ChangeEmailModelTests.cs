using Churchee.Module.Identity.Models;
using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Tests.Models
{
    public class ChangeEmailModelTests
    {
        [Fact]
        public void Should_Have_Error_When_NewEmail_Is_Empty()
        {
            // Arrange
            var model = new ChangeEmailModel { NewEmail = string.Empty };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The New email field is required.");
        }

        [Fact]
        public void Should_Have_Error_When_NewEmail_Is_Invalid()
        {
            // Arrange
            var model = new ChangeEmailModel { NewEmail = "invalid-email" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The New email field is not a valid e-mail address.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_NewEmail_Is_Valid()
        {
            // Arrange
            var model = new ChangeEmailModel { NewEmail = "test@example.com" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }
    }
}

