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

                var countsTask = GetCounts(start, cts.Token);

                await Task.WhenAll(referralTask, devicesTask, pagesOverTimeTask, topPagesTask, countsTask);

                var referral = await referralTask;
                var devices = await devicesTask;
                var pagesOverTime = await pagesOverTimeTask;
                var topPages = await topPagesTask;
                var totals = await countsTask;

                var response = new GetDashboardDataResponse()
                {
                    ReferralSource = referral ?? [],
                    Devices = devices ?? [],
                    PagesOverTime = pagesOverTime ?? [],
                    TopPages = topPages ?? [],
                    UniqueVisitors = totals?.UniqueVisitors ?? 0,
                    ReturningVisitors = totals?.ReturningVisitors ?? 0,
                    TotalPageViews = totals?.PageViews ?? 0
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

        private static DateOnly GetStartDate(GetDashboardDataQuery request)
        {
            return DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-request.Days);
        }

        private async Task<GetDashboardDataResponseItem[]> GetPagesOverTime(DateOnly start, CancellationToken cancellationToken)
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

        private async Task<Totals> GetCounts(DateOnly start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var totals = await dataStore
                .GetRepository<AnalyticsDaily>()
                .FirstOrDefaultAsync(new DailyAnalyticsSpecification(start),
                s => new Totals
                {
                    PageViews = s.PageViews,
                    UniqueVisitors = s.UniqueVisitors,
                    ReturningVisitors = s.ReturningVisitors
                }, cancellationToken);

            return totals ?? new Totals();
        }

        private async Task<GetDashboardDataResponseItem[]> GetTopPages(DateOnly start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            var data = await dataStore.GetRepository<AnalyticsDailyPages>().GetListAsync(new AnalyticsDailyAfterDateSpecification(start),
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

        private async Task<GetDashboardDataResponseItem[]> GetDevices(DateOnly start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            int total = await dataStore.GetRepository<AnalyticsDailyDevices>().CountAsync(new DailyDevicesSpecification(start), cancellationToken);

            if (total == 0)
            {
                return [];
            }

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

        private async Task<GetDashboardDataResponseItem[]> GetReferralSources(DateOnly start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var data = await dataStore.GetRepository<AnalyticsDailyReferrers>().GetListAsync(new ReferralSourcesSpecification(start),
                groupBy: g => g.Referrer,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            int total = await dataStore.GetRepository<AnalyticsDailyReferrers>().CountAsync(new ReferralSourcesSpecification(start), cancellationToken);

            if (total == 0)
            {
                return [];
            }

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
