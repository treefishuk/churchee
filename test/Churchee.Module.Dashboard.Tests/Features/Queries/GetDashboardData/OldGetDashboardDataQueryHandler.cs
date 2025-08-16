using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Dashboard.Features.Queries.GetDashboardData
{
    public class OldGetDashboardDataQueryHandler
    {
        private readonly IDataStore _dataStore;

        public OldGetDashboardDataQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<GetDashboardDataResponse> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var start = GetStartDate(request);

            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new GetPageViewDataForRange(start), cancellationToken);

            var pastVisitors = await _dataStore.GetRepository<PageView>().GetDistinctListAsync(new GetIpsBeforeDateSpecification(start), s => s.IpAddress, cancellationToken);

            var response = new GetDashboardDataResponse()
            {
                ReferralSource = GetReferralSources(data),
                Devices = GetDevices(data),
                PagesOverTime = GetPagesOverTime(data),
                TopPages = GetTopPages(data),
                UniqueVisitors = GetUniqueVisitors(data, pastVisitors),
                ReturningVisitors = GetReturnVisitors(data, pastVisitors),
                TotalPageViews = data.Count
            };

            return response;
        }

        private static DateTime GetStartDate(GetDashboardDataQuery request)
        {
            var startOfTheDay = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);

            int negativeNumber = request.Days * -1;

            var start = startOfTheDay.AddDays(negativeNumber);
            return start;
        }

        private static int GetUniqueVisitors(List<PageView> recentPageViews, List<string> pastVisitorIps)
        {
            var uniqueVisitors = recentPageViews
                .GroupBy(record => record.IpAddress)
                .Select(group => group.Key);

            var newVisitors = uniqueVisitors
                .Where(ip => !pastVisitorIps.Contains(ip))
                .Distinct();

            return newVisitors.Count();
        }

        private static int GetReturnVisitors(List<PageView> recentPageViews, List<string> pastVisitorIps)
        {
            var uniqueVisitors = recentPageViews
                .GroupBy(record => record.IpAddress)
                .Select(group => group.Key);

            var returnVisitors = uniqueVisitors
                .Where(ip => pastVisitorIps.Contains(ip))
                .Distinct();

            return returnVisitors.Count();
        }

        private static GetDashboardDataResponseItem[] GetTopPages(List<PageView> data)
        {
            return data.GroupBy(x => x.Url).Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = x.Count()
            }).OrderByDescending(x => x.Count).Take(5).ToArray();
        }

        private static GetDashboardDataResponseItem[] GetPagesOverTime(List<PageView> data)
        {
            return data.Select(s => new { Hour = s.ViewedAt.ToString("HH") }).GroupBy(x => x.Hour).Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key + ":00",
                Count = x.Count()
            }).OrderBy(x => x.Name).ToArray();
        }

        private static GetDashboardDataResponseItem[] GetDevices(List<PageView> data)
        {
            int totalRequestsAfterDate = data.Count;

            return data
                .GroupBy(record => new { record.Device })
                .Select(group => new GetDashboardDataResponseItem
                {
                    Name = group.Key.Device,
                    Count = Math.Round((double)group.Count() / totalRequestsAfterDate * 100, 2)
                })
                .Distinct()
                .ToArray();
        }

        private static GetDashboardDataResponseItem[] GetReferralSources(List<PageView> data)
        {
            var filteredList = data
                .Where(w => !string.IsNullOrEmpty(w.Referrer))
                .Select(s => new
                {
                    Referrer = GetHost(s.Referrer),
                })
                .Where(w => w.Referrer != "Invalid URL"
                        && !w.Referrer.StartsWith("/.")
                        )
                .ToList();

            // Calculate the total number of records
            int total = filteredList.Count;

            var groupedByReferrer = filteredList
                .GroupBy(x => new { x.Referrer }).ToList();

            return groupedByReferrer.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key.Referrer,
                Count = Math.Round((double)x.Count() / total * 100, 2)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToArray();
        }

        private static string GetHost(string referrer)
        {
            try
            {
                return new Uri(referrer).Host;
            }
            catch (UriFormatException)
            {
                // Handle the invalid URL situation here
                // For example, return a default value or a specific string
                return "Invalid URL";
            }
        }


    }
}
