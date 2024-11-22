using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Dashboard.Features.Queries.GetDashboardData
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, GetDashboardDataResponse>
    {
        private readonly IDataStore _dataStore;

        public GetDashboardDataQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<GetDashboardDataResponse> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var start = GetStartDate(request);

            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new GetPageViewDataForRange(start), cancellationToken);

            var pastVisitors = await _dataStore.GetRepository<PageView>().GetListAsync(new GetIpsBeforeDateSpecification(start), s => s.IpAddress, cancellationToken);

            var response = new GetDashboardDataResponse()
            {
                ReferralSource = GetReferralSources(data),
                Devices = GetDevices(data),
                PagesOverTime = GetPagesOverTime(data),
                TopPages = GetTopPages(data),
                UniqueVisitors = await GetUniqueVisitors(data, pastVisitors, cancellationToken),
                ReturningVisitors = await GetReturnVisitors(data, pastVisitors, cancellationToken),
                TotalPageViews = data.Count
            };

            return response;
        }

        private static DateTime GetStartDate(GetDashboardDataQuery request)
        {
            var startOfTheDay = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, DateTime.UtcNow.Day);

            int negativeNumber = (request.Days * -1);

            DateTime start = startOfTheDay.AddDays(negativeNumber);
            return start;
        }

        private async Task<int> GetUniqueVisitors(List<PageView> recentPageViews, List<string> pastVisitorIps, CancellationToken cancellationToken)
        {
            var uniqueVisitors = recentPageViews
                .GroupBy(record => record.IpAddress)
                .Select(group => group.Key);

            var newVisitors = uniqueVisitors
                .Where(ip => !pastVisitorIps.Contains(ip))
                .Distinct();

            return newVisitors.Count();
        }

        private async Task<int> GetReturnVisitors(List<PageView> recentPageViews, List<string> pastVisitorIps, CancellationToken cancellationToken)
        {
            var uniqueVisitors = recentPageViews
                .GroupBy(record => record.IpAddress)
                .Select(group => group.Key);

            var returnVisitors = uniqueVisitors
                .Where(ip => pastVisitorIps.Contains(ip))
                .Distinct();

            return returnVisitors.Count();
        }

        private GetDashboardDataResponseItem[] GetTopPages(List<PageView> data)
        {
            return data.GroupBy(x => x.Url).Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = x.Count()
            }).OrderByDescending(x => x.Count).Take(5).ToArray();
        }

        private GetDashboardDataResponseItem[] GetPagesOverTime(List<PageView> data)
        {
            return data.Select(s => new { Hour = s.ViewedAt.ToString("HH") }).GroupBy(x => x.Hour).Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key + ":00",
                Count = x.Count()
            }).OrderBy(x => x.Name).ToArray();
        }

        private GetDashboardDataResponseItem[] GetDevices(List<PageView> data)
        {
            var totalRequestsAfterDate = data.Count;

            return data
                .GroupBy(record => new { record.Device })
                .Select(group => new GetDashboardDataResponseItem
                {
                    Name = group.Key.Device,
                    Count = Math.Round(((double)group.Count() / totalRequestsAfterDate) * 100, 2)
                })
                .Distinct()
                .ToArray();
        }

        private GetDashboardDataResponseItem[] GetReferralSources(List<PageView> data)
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
            var total = filteredList.Count;

            var groupedByReferrer = filteredList
                .GroupBy(x => new { x.Referrer }).ToList();

            return groupedByReferrer.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key.Referrer,
                Count = Math.Round(((double)x.Count() / total) * 100, 2)
            })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToArray();
        }

        string GetHost(string referrer)
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
