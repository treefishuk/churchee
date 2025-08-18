using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using FluentAssertions;
using System.Linq.Expressions;

namespace Churchee.Module.Dashboard.Tests.Specifications
{
    public class GetIpsBeforeDateSpecificationTests
    {
        [Fact]
        public void GetIpsBeforeDateSpecification_Constructor_HasWhereStatement()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsBeforeDateSpecification(date);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            criteria.Should().NotBeNull();
            criteria?.Filter.Should().NotBeNull();
            criteria?.Filter.Should().BeAssignableTo<Expression<Func<PageView, bool>>>();
        }

        [Fact]
        public void GetIpsBeforeDateSpecification_ReturnsTrueWhenWhenDateInRange()
        {
            // Arrange
            var date = new DateTime(2024, 10, 1);

            // Act
            var spec = new PageViewsBeforeDateSpecification(date);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();
            PageView testPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2023, 9, 30) };
            compiledCriteria(testPageView).Should().BeTrue();
        }

        [Fact]
        public void GetIpsBeforeDateSpecification_ReturnsFalseWhenWhenDateOutOfRange()
        {
            // Arrange
            var date = new DateTime(2024, 10, 1);

            // Act
            var spec = new PageViewsBeforeDateSpecification(date);

            // Assert
            var criteria = spec.WhereExpressions.FirstOrDefault();
            var compiledCriteria = criteria!.Filter.Compile();
            var testPageView = new PageView(Guid.NewGuid()) { ViewedAt = new DateTime(2025, 9, 30) };
            compiledCriteria(testPageView).Should().BeFalse();
        }

        [Fact]
        public void GetIpsBeforeDateSpecification_Constructor_ShouldSetAsNoTracking()
        {
            // Arrange
            var date = new DateTime(2023, 10, 1);

            // Act
            var spec = new PageViewsBeforeDateSpecification(date);

            // Assert
            spec.AsNoTracking.Should().BeTrue();
        }
    }
}

