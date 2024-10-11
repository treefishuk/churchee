using Churchee.Module.Identity.Features.Roles.Queries;
using FluentAssertions;

namespace Churchee.Module.Identity.Tests.Features.Roles.Queries.GetAllSelectableRolesForUser
{
    public class GetAllSelectableRolesForUserTests
    {
        [Fact]
        public void Constructor_ShouldInitializeUserIdProperty()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var query = new GetAllSelectableRolesForUserQuery(userId);

            // Assert
            query.UserId.Should().Be(userId);

        }
    }
}