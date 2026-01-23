using Churchee.Common.ValueTypes;
using Churchee.Module.Identity.Models;
using Churchee.Test.Helpers.Validation;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Module.Identity.Tests.Models
{
    public class EditContributorModelTests
    {
        private readonly MultiSelect _roles;

        public EditContributorModelTests()
        {
            var items = new List<MultiSelectItem>
            {
                new MultiSelectItem(Guid.NewGuid(), "Role1"),
                new MultiSelectItem(Guid.NewGuid(), "Role2")
            };
            _roles = new MultiSelect(items);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Roles_Are_Valid()
        {
            // Arrange
            var model = new EditContributorModel(_roles);
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