using Churchee.Module.Dashboard.Features.Queries;
using FluentAssertions;

namespace Churchee.Module.Dashboard.Tests.Features.Queries
{
    public class GetDashboardDataQueryTests
    {
        [Fact]
        public void Constructor_ShouldSetDaysProperty()
        {
            // Arrange
            int days = 7;

            // Act
            var query = new GetDashboardDataQuery(days);

            // Assert
            query.Days.Should().Be(days);
        }

        [Fact]
        public void DaysProperty_ShouldBeSettable()
        {
            // Arrange
            var query = new GetDashboardDataQuery(7);
            int newDays = 10;

            // Act
            query.Days = newDays;

            // Assert
            query.Days.Should().Be(newDays);
        }
    }
}
