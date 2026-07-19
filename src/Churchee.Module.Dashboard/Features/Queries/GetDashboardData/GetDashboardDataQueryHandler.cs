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
                var uniqueVisitorsTask = GetUniqueVisitors(start, cts.Token);
                var returningVisitorsTask = GetReturnVisitors(start, cts.Token);
                var totalPageViewsTask = GetTotalViews(start, cts.Token);

                await Task.WhenAll(referralTask, devicesTask, pagesOverTimeTask, topPagesTask,
                   uniqueVisitorsTask, returningVisitorsTask, totalPageViewsTask);

                var response = new GetDashboardDataResponse()
                {
                    ReferralSource = referralTask.Result,
                    Devices = devicesTask.Result,
                    PagesOverTime = pagesOverTimeTask.Result,
                    TopPages = topPagesTask.Result,
                    UniqueVisitors = uniqueVisitorsTask.Result,
                    ReturningVisitors = returningVisitorsTask.Result,
                    TotalPageViews = totalPageViewsTask.Result
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
            var data = await dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
            groupBy: g => g.ViewedAtHour,
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

        private async Task<int> GetTotalViews(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            return await dataStore.GetRepository<PageView>().CountAsync(new PageViewsAfterDateSpecification(start), cancellationToken);
        }

        private async Task<int> GetReturnVisitors(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            var inQuery = dataStore.GetRepository<PageView>().ApplySpecification(new PageViewsBeforeDateSpecification(start)).Select(s => s.IpAddress).Distinct();

            int returnVisitors = await dataStore.GetRepository<PageView>().GetDistinctCountAsync(new ReturnVisitorsSpecification(start, inQuery),
                selector: s => s.IpAddress,
                cancellationToken: cancellationToken);

            return returnVisitors;
        }

        private async Task<int> GetUniqueVisitors(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();

            var notInQuery = dataStore.GetRepository<PageView>().ApplySpecification(new PageViewsBeforeDateSpecification(start)).Select(s => s.IpAddress).Distinct();

            int returnVisitors = await dataStore.GetRepository<PageView>().GetDistinctCountAsync(new UniqueVisitorsSpecification(start, notInQuery),
                selector: s => s.IpAddress,
                cancellationToken: cancellationToken);

            return returnVisitors;
        }

        private async Task<GetDashboardDataResponseItem[]> GetTopPages(DateTime start, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dataStore = scope.ServiceProvider.GetRequiredService<IDataStore>();
            var data = await dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
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

            int total = await dataStore.GetRepository<PageView>().CountAsync(new PageViewsAfterDateSpecification(start), cancellationToken);

            var data = await dataStore.GetRepository<PageView>().GetListAsync(new PageViewsAfterDateSpecification(start),
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

            int total = await dataStore.GetRepository<PageView>().CountAsync(new ReferralSourcesSpecification(start), cancellationToken);

            var data = await dataStore.GetRepository<PageView>().GetListAsync(new ReferralSourcesSpecification(start),
                groupBy: g => g.Referrer,
                selector: s => new { s.Key, Count = s.Count() },
                take: 5,
                cancellationToken: cancellationToken);

            return [.. data.Select(x => new GetDashboardDataResponseItem
            {
                Name = x.Key,
                Count = Math.Round((double)x.Count / total * 100, 2)
            })];
        }
    }
}
