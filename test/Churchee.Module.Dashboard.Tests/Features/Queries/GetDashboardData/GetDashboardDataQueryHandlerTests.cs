using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Features.Queries;
using Churchee.Module.Dashboard.Features.Queries.GetDashboardData;
using Churchee.Module.Dashboard.Specifications;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Dashboard.Tests.Features.Queries
{
    public class GetDashboardDataQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly GetDashboardDataQueryHandler _handler;

        public GetDashboardDataQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _handler = new GetDashboardDataQueryHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectResponse()
        {
            // Arrange
            Guid appTenantId = Guid.NewGuid();
            var query = new GetDashboardDataQuery(7);
            var cancellationToken = new CancellationToken();

            var pageViews = new List<PageView>
            {
                new PageView(appTenantId) { IpAddress = "192.168.1.1", ViewedAt = DateTime.UtcNow.AddDays(-1), Device = "Device1", Url = "/page1", Referrer = "http://referrer1.com" },
                new PageView(appTenantId) { IpAddress = "192.168.1.2", ViewedAt = DateTime.UtcNow.AddDays(-2), Device = "Device2", Url = "/page2", Referrer = "http://referrer2.com" }
            };

            var pastVisitorIps = new List<string> { "192.168.1.3" };

            _dataStoreMock.Setup(ds => ds.GetRepository<PageView>().GetListAsync(It.IsAny<GetPageViewDataForRange>(), cancellationToken))
                .ReturnsAsync(pageViews);

            _dataStoreMock.Setup(ds => ds.GetRepository<PageView>().GetListAsync(It.IsAny<GetIpsBeforeDateSpecification>(), It.IsAny<Expression<Func<PageView, string>>>(), cancellationToken))
                .ReturnsAsync(pastVisitorIps);

            // Act
            var response = await _handler.Handle(query, cancellationToken);

            // Assert
            response.Should().NotBeNull();
            response.TotalPageViews.Should().Be(pageViews.Count);
            response.UniqueVisitors.Should().Be(2); // Both IPs are unique and not in pastVisitorIps
            response.ReturningVisitors.Should().Be(0); // No returning visitors
            response.Devices.Should().HaveCount(2);
            response.TopPages.Should().HaveCount(2);
            response.PagesOverTime.Should().NotBeEmpty();
            response.ReferralSource.Should().HaveCount(2);
        }
    }
}




