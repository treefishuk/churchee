using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using FluentAssertions;
using System.Linq.Expressions;

namespace Churchee.Module.Dashboard.Tests.Specifications
{

    public class GetPageViewDataForRangeTests
    {
        [Fact]
        public void GetPageViewDataForRange_Constructor_ShouldSetCorrectCriteria_ForValidPageView()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();

            var compiledCriteria = criteria!.Filter.Compile();
            var validPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2023, 10, 2), Device = "Device1", UserAgent = "UserAgent1" };
            compiledCriteria(validPageView).Should().BeTrue();
        }

        [Fact]
        public void GetPageViewDataForRange_Constructor_ShouldSetCorrectCriteria_ForInvalidPageView_BeforeStartDate()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();

            var compiledCriteria = criteria!.Filter.Compile();
            var invalidPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2023, 9, 30), Device = "Device1", UserAgent = "UserAgent1" };
            compiledCriteria(invalidPageView).Should().BeFalse();
        }

        [Fact]
        public void GetPageViewDataForRange_Constructor_ShouldSetCorrectCriteria_ForInvalidPageView_EmptyDevice()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();

            var compiledCriteria = criteria!.Filter.Compile();
            var invalidPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2023, 10, 2), Device = "", UserAgent = "UserAgent1" };
            compiledCriteria(invalidPageView).Should().BeFalse();
        }

        [Fact]
        public void GetPageViewDataForRange_Constructor_ShouldSetCorrectCriteria_ForInvalidPageView_EmptyUserAgent()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();

            var compiledCriteria = criteria!.Filter.Compile();
            var invalidPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2023, 10, 2), Device = "Device1", UserAgent = "" };
            compiledCriteria(invalidPageView).Should().BeFalse();
        }

        [Fact]
        public void GetPageViewDataForRange_Constructor_ShouldSetAsNoTracking()
        {
            // Arrange
            var startDate = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsAfterDateSpecification(startDate);

            // Assert
            spec.AsNoTracking.Should().BeTrue();
        }
    }

}
