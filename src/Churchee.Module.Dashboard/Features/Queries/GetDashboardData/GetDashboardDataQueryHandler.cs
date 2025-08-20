using Churchee.Common.Storage;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using DeviceDetectorNET;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Dashboard.Features.Queries.GetDashboardData
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, GetDashboardDataResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ILogger _logger;

        public GetDashboardDataQueryHandler(IDataStore dataStore, ILogger<GetDashboardDataQueryHandler> logger)
        {
            _dataStore = dataStore;
            _logger = logger;
        }

        public async Task<GetDashboardDataResponse> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                cts.CancelAfter(TimeSpan.FromSeconds(15)); // Set a 15s timeout

                var start = GetStartDate(request);

                var referralSource = await GetReferralSources(start, request, cts.Token);
                var devices = await GetDevices(start, request, cts.Token); //32ms
                var pagesOverTime = await GetPagesOverTime(start, request, cts.Token);
                var topPages = await GetTopPages(start, request, cts.Token);
                int uniqueVisitors = await GetUniqueVisitors(start, request, cts.Token);
                int returningVisitors = await GetReturnVisitors(start, request, cts.Token);
                int totalPageViews = await GetTotalViews(start, cts.Token);

                var response = new GetDashboardDataResponse()
                {
                    ReferralSource = referralSource,
                    Devices = devices,
                    PagesOverTime = pagesOverTime,
                    TopPages = topPages,
                    UniqueVisitors = uniqueVisitors,
                    ReturningVisitors = returningVisitors,
                    TotalPageViews = totalPageViews
                };

                return response;
            }
            catch (OperationCanceledException)
            {
                return new GetDashboardDataResponse() { ErrorMessage = "Data Took to long to return" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the GetDashboardDataQuery.");

                return new GetDashboardDataResponse() { ErrorMessage = "An unknown error occurred while returning the data" };
            }
        }

        private static DateTime GetStartDate(GetDashboardDataQuery request)
        {
            var startOfTheDay = new DateTime(DateTime.UtcNow.Date.Year, DateTime.UtcNow.Date.Month, DateTime.UtcNow.Day, 0, 0, 0, DateTimeKind.Utc);

            int negativeNumber = request.Days * -1;

            var start = startOfTheDay.AddDays(negativeNumber);

            return start;
        }

        private async Task<GetDashboardDataResponseItem[]> GetPagesOverTime(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
                groupBy: g => g.ViewedAt.Hour,
                selector: s => new
                {
                    s.Key,
                    Count = s.Count()
                },
                cancellationToken: cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key + ":00",
                Count = x.Count
            }).OrderBy(o => o.Name)];
        }

        private async Task<int> GetTotalViews(DateTime start, CancellationToken cancellationToken)
        {
            return await _dataStore.GetRepository<PageView>().CountAsync(new PageViewsAfterDateSpecification(start), cancellationToken);
        }

        private async Task<int> GetReturnVisitors(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {

            var inQuery = _dataStore.GetRepository<PageView>().ApplySpecification(new PageViewsBeforeDateSpecification(start)).Select(s => s.IpAddress).Distinct();

            int returnVisitors = await _dataStore.GetRepository<PageView>().GetDistinctCountAsync(new ReturnVisitorsSpecification(start, inQuery),
                selector: s => s.IpAddress,
                cancellationToken: cancellationToken);

            return returnVisitors;
        }

        private async Task<int> GetUniqueVisitors(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var notInQuery = _dataStore.GetRepository<PageView>().ApplySpecification(new PageViewsBeforeDateSpecification(start)).Select(s => s.IpAddress).Distinct();

            int returnVisitors = await _dataStore.GetRepository<PageView>().GetDistinctCountAsync(new UniqueVisitorsSpecification(start, notInQuery),
                selector: s => s.IpAddress,
                cancellationToken: cancellationToken);

            return returnVisitors;
        }

        private async Task<GetDashboardDataResponseItem[]> GetTopPages(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
                groupBy: g => g.Url,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = x.Count
            })];
        }

        private async Task<GetDashboardDataResponseItem[]> GetDevices(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            int total = await _dataStore.GetRepository<PageView>().CountAsync(new PageViewsAfterDateSpecification(start), cancellationToken);

            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
                groupBy: g => g.Device,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = Math.Round((double)x.Count / total * 100, 2)
            })];

        }

        private async Task<GetDashboardDataResponseItem[]> GetReferralSources(DateTime start, GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            int total = await _dataStore.GetRepository<PageView>().CountAsync(new ReferralSourcesSpecification(start), cancellationToken);

            var data = await _dataStore.GetRepository<PageView>().GetListAsync(new ReferralSourcesSpecification(start),
                groupBy: g => g.Referrer,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = GetHost(x.Key),
                Count = Math.Round((double)x.Count / total * 100, 2)
            })];
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
