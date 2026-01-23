using Churchee.Module.Tenancy.Specifications;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Tenancy.Tests.Specifications
{
    public class ApplicationHostsByApplicationIdSpecificationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSpecificationWithCorrectCriteria()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new ApplicationHostsByApplicationIdSpecification(applicationTenantId);

            // Assert
            var criteria = specification.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.ToString().Should().ContainAll("Id");
        }

        [Fact]
        public void Constructor_ShouldIgnoreQueryFilters()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new ApplicationHostsByApplicationIdSpecification(applicationTenantId);

            // Assert
            specification.IgnoreQueryFilters.Should().BeTrue();
        }


    }
}
