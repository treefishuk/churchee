using Churchee.Module.Identity.Models;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Tests.Models
{
    public class EnableMultiFactorModelTests
    {
        [Fact]
        public void Should_Have_Error_When_Code_Is_Empty()
        {
            // Arrange
            var model = new EnableMultiFactorModel { Code = string.Empty };
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Code field is required.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Code_Is_Valid()
        {
            // Arrange
            var model = new EnableMultiFactorModel { Code = "123456" };
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
