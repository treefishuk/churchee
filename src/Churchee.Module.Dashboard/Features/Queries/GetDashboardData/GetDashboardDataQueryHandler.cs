using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;
using DeviceDetectorNET;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Dashboard.Features.Queries.GetDashboardData
{
    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, GetDashboardDataResponse>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger _logger;

        public GetDashboardDataQueryHandler(IServiceScopeFactory scopeFactory, ILogger<GetDashboardDataQueryHandler> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<GetDashboardDataResponse> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

                cts.CancelAfter(TimeSpan.FromSeconds(15)); // Set a 15s timeout

                var start = GetStartDate(request);

                var referralTask = GetReferralSources(start, cts.Token);
                var devicesTask = GetDevices(start, cts.Token); //32ms
                var pagesOverTimeTask = GetPagesOverTime(start, cts.Token);
                var topPagesTask = GetTopPages(start, cts.Token);

                var counts = GetCounts(start, cts.Token);

                await Task.WhenAll(referralTask, devicesTask, pagesOverTimeTask, topPagesTask, counts);

                var response = new GetDashboardDataResponse()
                {
                    ReferralSource = referralTask.Result,
                    Devices = devicesTask.Result,
                    PagesOverTime = pagesOverTimeTask.Result,
                    TopPages = topPagesTask.Result,
                    UniqueVisitors = counts.Result.UniqueVisitors,
                    ReturningVisitors = counts.Result.ReturningVisitors,
                    TotalPageViews = counts.Result.PageViews
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

        private async Task<GetDashboardDataResponseItem[]> GetPagesOverTime(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            var data = await dataStore.GetRepository<AnalyticsHourly>().GetListAsync(new HourlyPageViewsAfterDateSpecification(start),
            groupBy: g => g.HourOfDay,
            selector: s => new
            {
                s.Key,
                Count = s.Count()
            },
            cancellationToken: cancellationToken);

            return [.. data.OrderBy(x => x.Key).Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key + ":00",
                Count = x.Count
            })];
        }

        private async Task<Totals> GetCounts(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            return await dataStore
                .GetRepository<AnalyticsDaily>()
                .FirstOrDefaultAsync(new DailyAnalyticsSpecification(start),
                s => new Totals
                {
                    PageViews = s.PageViews,
                    UniqueVisitors = s.UniqueVisitors,
                    ReturningVisitors = s.ReturningVisitors
                }, cancellationToken);
        }

        private async Task<GetDashboardDataResponseItem[]> GetTopPages(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            var data = await dataStore.GetRepository<AnalyticsDailyPages>().GetListAsync(new PageViewsAfterDateSpecification(start),
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

        private async Task<GetDashboardDataResponseItem[]> GetDevices(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            int total = await dataStore.GetRepository<AnalyticsDailyDevices>().CountAsync(new DailyDevicesSpecification(start), cancellationToken);

            var data = await dataStore.GetRepository<AnalyticsDailyDevices>().GetListAsync(new DailyDevicesSpecification(start),
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

        private async Task<GetDashboardDataResponseItem[]> GetReferralSources(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var data = await dataStore.GetRepository<AnalyticsDailyReferrers>().GetListAsync(new ReferralSourcesSpecification(start),
                groupBy: g => g.Referrer,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            int total = await dataStore.GetRepository<AnalyticsDailyReferrers>().CountAsync(new ReferralSourcesSpecification(start), cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = Math.Round((double)x.Count / total * 100, 2)
            })];
        }

        private class Totals
        {
            public int PageViews { get; set; }
            public int UniqueVisitors { get; set; }
            public int ReturningVisitors { get; set; }
        }
    }
}
