using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Test.Helpers.Validation;
using Xunit;

namespace Churchee.Module.Tokens.Tests.Specifications
{
    public class GetTokenByKeySpecificationTests
    {
        [Fact]
        public void Constructor_ShouldInitializeQueryCorrectly()
        {
            // Arrange
            var key = "TestKey";
            var applicationTenantId = Guid.NewGuid();

            // Act
            var spec = new GetTokenByKeySpecification(key, applicationTenantId);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault()?.Filter;
            criteria.Should().NotBeNull();
            criteria?.Compile()(new Token(applicationTenantId, key, "TestValue")).Should().BeTrue();
            criteria?.Compile()(new Token(applicationTenantId, "DifferentKey", "TestValue")).Should().BeFalse();
            criteria?.Compile()(new Token(Guid.NewGuid(), key, "TestValue")).Should().BeFalse();
        }

        [Fact]
        public void Constructor_ShouldIgnoreQueryFilters()
        {
            // Arrange
            var key = "TestKey";
            var applicationTenantId = Guid.NewGuid();

            // Act
            var spec = new GetTokenByKeySpecification(key, applicationTenantId);

            // Assert
            spec.IgnoreQueryFilters.Should().BeTrue();
        }

        [Fact]
        public void Constructor_ShouldSetAsNoTracking()
        {
            // Arrange
            var key = "TestKey";
            var applicationTenantId = Guid.NewGuid();

            // Act
            var spec = new GetTokenByKeySpecification(key, applicationTenantId);

            // Assert
            spec.AsNoTracking.Should().BeTrue();
        }
    }
}
