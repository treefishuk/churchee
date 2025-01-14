using Churchee.Module.Identity.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Tests.Models
{
    public class ChangePasswordModelTests
    {
        [Fact]
        public void Should_Have_Error_When_OldPassword_Is_Empty()
        {
            // Arrange
            var model = new ChangePasswordModel { OldPassword = string.Empty };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Current password field is required.");
        }

        [Fact]
        public void Should_Have_Error_When_NewPassword_Is_Too_Short()
        {
            // Arrange
            var model = new ChangePasswordModel { NewPassword = "short" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The New password must be at least 6 and at max 100 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_ConfirmPassword_Does_Not_Match()
        {
            // Arrange
            var model = new ChangePasswordModel { NewPassword = "validPassword123", ConfirmPassword = "differentPassword" };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The new password and confirmation password do not match.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var model = new ChangePasswordModel
            {
                OldPassword = "oldPassword123",
                NewPassword = "newPassword123456!",
                ConfirmPassword = "newPassword123456!"
            };

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


