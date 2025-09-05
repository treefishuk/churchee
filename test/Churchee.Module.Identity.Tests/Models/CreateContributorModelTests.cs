using Churchee.Common.ValueTypes;
using Churchee.Module.Identity.Models;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Tests.Models
{
    public class CreateContributorModelTests
    {
        private readonly MultiSelect _roles;

        public CreateContributorModelTests()
        {
            var items = new List<MultiSelectItem>
            {
                new MultiSelectItem(Guid.NewGuid(), "Role1"),
                new MultiSelectItem(Guid.NewGuid(), "Role2")
            };
            _roles = new MultiSelect(items);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            // Arrange
            var model = new CreateContributorModel(_roles) { Email = string.Empty };
            var validationResults = new List<ValidationResult>();
            var validationContext = GetValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Email field is required.");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            // Arrange
            var model = new CreateContributorModel(_roles) { Email = "invalid-email" };
            var validationResults = new List<ValidationResult>();
            var validationContext = GetValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The Email field is not a valid e-mail address.");
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            // Arrange
            var model = new CreateContributorModel(_roles)
            {
                Email = "test@example.com",
                Password = "TooShort!22",
                ConfirmPassword = "TooShort!22"
            };
            var validationResults = new List<ValidationResult>();
            var validationContext = GetValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "Password must be at least 16 characters long.");
        }

        [Fact]
        public void Should_Have_Error_When_ConfirmPassword_Does_Not_Match()
        {
            // Arrange
            var model = new CreateContributorModel(_roles) { Password = "validPassword123", ConfirmPassword = "differentPassword" };
            var validationResults = new List<ValidationResult>();
            var validationContext = GetValidationContext(model);

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeFalse();
            validationResults.Should().ContainSingle(vr => vr.ErrorMessage == "The password and confirmation password do not match.");
        }

        [Fact]
        public void Should_Not_Have_Error_When_All_Fields_Are_Valid()
        {
            // Arrange
            var model = new CreateContributorModel(_roles)
            {
                Email = "test@example.com",
                Password = "validPassword123456!",
                ConfirmPassword = "validPassword123456!"
            };

            var validationContext = GetValidationContext(model);
            var validationResults = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

            // Assert
            isValid.Should().BeTrue();
            validationResults.Should().BeEmpty();
        }

        private static ValidationContext GetValidationContext(object model)
        {
            var identityOptions = Options.Create(new IdentityOptions
            {
                Password = new PasswordOptions
                {
                    RequireDigit = true,
                    RequiredLength = 16,
                    RequireLowercase = true,
                    RequireNonAlphanumeric = true,
                    RequireUppercase = true,
                    RequiredUniqueChars = 1
                }
            });

            var serviceProvider = new Mock<IServiceProvider>();

            serviceProvider
                .Setup(x => x.GetService(typeof(IOptions<IdentityOptions>)))
                .Returns(identityOptions);

            var validationContext = new ValidationContext(model, serviceProvider: serviceProvider.Object, items: null);

            return validationContext;

        }
    }
}
