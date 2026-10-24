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
            var startDate = new DateOnly(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<AnalyticsDailyPages, bool>>>();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_ReturnsTrue_WhenDateIsAfterStartDate()
        {
            // Arrange
            var startDate = new DateOnly(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();

            var validPageView = new AnalyticsDailyPages(Guid.NewGuid())
            {
                Date = new DateOnly(2023, 10, 2)
            };

            compiledCriteria(validPageView).Should().BeTrue();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_ReturnsFalse_WhenDateIsEqualToStartDate()
        {
            // Arrange
            var startDate = new DateOnly(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();

            var invalidPageView = new AnalyticsDailyPages(Guid.NewGuid())
            {
                Date = new DateOnly(2023, 10, 1)
            };

            compiledCriteria(invalidPageView).Should().BeFalse();
        }

        [Fact]
        public void PageViewsAfterDateSpecification_Constructor_ShouldSetAsNoTracking()
        {
            // Arrange
            var startDate = new DateOnly(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            spec.AsNoTracking.Should().BeTrue();
        }
    }
}