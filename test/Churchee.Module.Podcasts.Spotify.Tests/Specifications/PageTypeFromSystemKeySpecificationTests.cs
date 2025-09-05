using Churchee.Module.Podcasts.Spotify.Specifications;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Spotify.Tests.Specifications
{
    public class PageTypeFromSystemKeySpecificationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeSpecificationWithCorrectCriteria()
        {
            // Arrange
            var systemKey = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new PageTypeFromSystemKeySpecification(systemKey, applicationTenantId);

            // Assert
            var criteria = specification.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.ToString().Should().ContainAll("SystemKey", "ApplicationTenantId", "Deleted");
        }

        [Fact]
        public void Constructor_ShouldIgnoreQueryFilters()
        {
            // Arrange
            var systemKey = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();

            // Act
            var specification = new PageTypeFromSystemKeySpecification(systemKey, applicationTenantId);

            // Assert
            specification.IgnoreQueryFilters.Should().BeTrue();
        }
    }
}
