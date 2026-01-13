using Churchee.Module.Tenancy.Specifications;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Tenancy.Tests.Specifications
{
    public class ApplicationTenantByIdIncludingHostsSpecificationTests
    {

        [Fact]
        public void Constructor_ShouldInitializeSpecificationWithCorrectCriteria()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new ApplicationTenantByIdIncludingHostsSpecification(applicationTenantId);

            // Assert
            var criteria = specification.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.ToString().Should().ContainAll("Id");

            var includes = specification.IncludeExpressions.FirstOrDefault();
            includes.Should().NotBeNull();
            includes?.LambdaExpression.ToString().Should().ContainAll("Hosts");

        }

        [Fact]
        public void Constructor_ShouldIgnoreQueryFilters()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new ApplicationTenantByIdIncludingHostsSpecification(applicationTenantId);

            // Assert
            specification.IgnoreQueryFilters.Should().BeTrue();
        }

    }
}
