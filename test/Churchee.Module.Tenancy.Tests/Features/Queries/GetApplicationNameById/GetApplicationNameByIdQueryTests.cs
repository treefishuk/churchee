using Churchee.Module.Tenancy.Features.Churches.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Tenancy.Tests.Features.Queries.GetApplicationNameById
{
    public class GetApplicationNameByIdQueryTests
    {
        [Fact]
        public void GetApplicationNameByIdQuery_Should_Set_Properties_Correctly()
        {
            // Arrange
            var applicationId = Guid.NewGuid();

            // Act
            var query = new GetApplicationNameByIdQuery(applicationId);

            // Assert
            query.ApplicationTenantId.Should().Be(applicationId);
        }

    }
}
