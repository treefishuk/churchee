using Churchee.Module.Dashboard.Features.Queries;
using Churchee.Module.Dashboard.Features.Queries.GetDashboardData;
using FluentAssertions;

namespace Churchee.Module.Dashboard.Tests.Features.Queries
{
    public class GetDashboardDataResponseTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var response = new GetDashboardDataResponse();

            // Assert
            response.Devices.Count().Should().Be(0);
            response.TopPages.Count().Should().Be(0);
            response.PagesOverTime.Count().Should().Be(0);
            response.ReferralSource.Count().Should().Be(0);
            response.UniqueVisitors.Should().Be(0);
            response.ReturningVisitors.Should().Be(0);
            response.TotalPageViews.Should().Be(0);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var devices = new GetDashboardDataResponseItem[] { new GetDashboardDataResponseItem { Name = "Device1", Count = 100 } };
            var topPages = new GetDashboardDataResponseItem[] { new GetDashboardDataResponseItem { Name = "Page1", Count = 200 } };
            var pagesOverTime = new GetDashboardDataResponseItem[] { new GetDashboardDataResponseItem { Name = "PageOverTime1", Count = 300 } };
            var referralSource = new GetDashboardDataResponseItem[] { new GetDashboardDataResponseItem { Name = "Referral1", Count = 400 } };
            int uniqueVisitors = 500;
            int returningVisitors = 600;
            int totalPageViews = 700;

            // Act
            var response = new GetDashboardDataResponse
            {
                Devices = devices,
                TopPages = topPages,
                PagesOverTime = pagesOverTime,
                ReferralSource = referralSource,
                UniqueVisitors = uniqueVisitors,
                ReturningVisitors = returningVisitors,
                TotalPageViews = totalPageViews
            };

            // Assert
            response.Devices.Should().BeEquivalentTo(devices);
            response.TopPages.Should().BeEquivalentTo(topPages);
            response.PagesOverTime.Should().BeEquivalentTo(pagesOverTime);
            response.ReferralSource.Should().BeEquivalentTo(referralSource);
            response.UniqueVisitors.Should().Be(uniqueVisitors);
            response.ReturningVisitors.Should().Be(returningVisitors);
            response.TotalPageViews.Should().Be(totalPageViews);
        }
    }
}
