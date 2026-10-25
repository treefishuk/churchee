using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using Churchee.Test.Helpers.Validation;
using System.Linq.Expressions;

namespace Churchee.Module.Dashboard.Tests.Specifications
{
    public class PageViewsAfterDateSpecificationTests
    {
        [Fact]
        public void PageViewsAfterDateSpecification_Constructor_HasWhereStatement()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_ReturnsTrue_WhenDateIsAfterStartDate()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();

            var validPageView = new PageView(Guid.NewGuid())
            {
                ViewedAt = new DateTime(2023, 10, 2, 0, 0, 0, DateTimeKind.Utc)
            };

            compiledCriteria(validPageView).Should().BeTrue();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_ReturnsTrue_WhenDateIsEqualToStartDate()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();

            var invalidPageView = new PageView(Guid.NewGuid())
            {
                ViewedAt = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc)
            };

            compiledCriteria(invalidPageView).Should().BeTrue();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_Constructor_ShouldSetAsNoTracking()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1, 0, 0, 0, DateTimeKind.Utc);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            spec.AsNoTracking.Should().BeTrue();
        }
    }
}