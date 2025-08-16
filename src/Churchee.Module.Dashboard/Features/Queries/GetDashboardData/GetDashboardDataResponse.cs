using Churchee.Module.Dashboard.Features.Queries.GetDashboardData;

namespace Churchee.Module.Dashboard.Features.Queries
{
    public class GetDashboardDataResponse
    {
        public GetDashboardDataResponse()
        {
            Devices = [];
            TopPages = [];
            PagesOverTime = [];
            ReferralSource = [];
            UniqueVisitors = 0;
            ReturningVisitors = 0;
            TotalPageViews = 0;
            ErrorMessage = string.Empty;
        }

        public GetDashboardDataResponseItem[] Devices { get; set; }
        public GetDashboardDataResponseItem[] TopPages { get; set; }
        public GetDashboardDataResponseItem[] PagesOverTime { get; set; }
        public GetDashboardDataResponseItem[] ReferralSource { get; set; }

        public int UniqueVisitors { get; set; }

        public int ReturningVisitors { get; set; }

        public int TotalPageViews { get; set; }

        public string ErrorMessage { get; set; }

    }
}
