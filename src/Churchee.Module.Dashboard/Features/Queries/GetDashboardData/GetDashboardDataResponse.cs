using Churchee.Module.Dashboard.Features.Queries.GetDashboardData;

namespace Churchee.Module.Dashboard.Features.Queries
{
    public class GetDashboardDataResponse
    {
        public GetDashboardDataResponseItem[] Devices { get; set; }
        public GetDashboardDataResponseItem[] TopPages { get; set; }
        public GetDashboardDataResponseItem[] PagesOverTime { get; set; }
        public GetDashboardDataResponseItem[] ReferralSource { get; set; }

        public int UniqueVisitors { get; set; }

        public int ReturningVisitors { get; set; }

        public int TotalPageViews { get; set; }

    }
}
