using Churchee.Module.Events.Specifications;
using Churchee.Module.Identity.Entities;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Identity.Tests.Specifications
{
    public class SelectableRolesSpecificationTests
    {

        [Fact]
        public void SelectableRolesSpecification_Should_Return_Only_Selectable_Roles()
        {
            // Arrange
            var roles = new List<ApplicationRole>
            {
                new ApplicationRole(Guid.NewGuid(), "Role1"){ Selectable = true },
                new ApplicationRole(Guid.NewGuid(), "Role2"){ Selectable = false },
                new ApplicationRole(Guid.NewGuid(), "Role3"){ Selectable = true },
                new ApplicationRole(Guid.NewGuid(), "Role4"){ Selectable = false }
            };

            var spec = new SelectableRolesSpecification();

            // Act
            var result = spec.Evaluate(roles);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.True(r.Selectable));
        }

    }
}
