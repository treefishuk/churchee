using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;

using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Specifications;

namespace Churchee.Module.Dashboard.Jobs
{
    public class PopulateAnalyticsJob : ISystemJob
    {
        private readonly IDataStore _dataStore;

        public PopulateAnalyticsJob(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var lastProcessedDate = await GetLastProcessedDate(cancellationToken);

            var lastProcessedDateTime = lastProcessedDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var pageViewRepository = _dataStore.GetRepository<PageView>();

            const int batchSize = 100;
            var skip = 0;

            while (true)
            {
                var datePageViews = await pageViewRepository
                    .GetListAsync(new PageViewsAfterDateBatchSpecification(lastProcessedDateTime, batchSize, skip),
                        x => new PageViewAggregate
                        {
                            ApplicationTenantId = x.ApplicationTenantId,
                            Date = DateOnly.FromDateTime(x.ViewedAt.Date),
                            IpAddress = string.IsNullOrWhiteSpace(x.IpAddress) ? string.Empty : x.IpAddress.Trim(),
                            Device = NormalizeDevice(x.Device),
                            Url = NormalizeUrl(x.Url),
                            Referrer = NormalizeReferrer(x.Referrer),
                            HourOfDay = x.ViewedAt.Hour
                        },
                        cancellationToken);

                if (datePageViews.Count == 0)
                {
                    break;
                }

                skip += datePageViews.Count;

                var previousIpsByTenant = new Dictionary<Guid, HashSet<string>>();

                foreach (var dateGroup in datePageViews
                    .GroupBy(x => x.Date)
                    .OrderBy(x => x.Key))
                {
                    var date = dateGroup.Key.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

                    foreach (var tenantGroup in dateGroup.GroupBy(x => x.ApplicationTenantId))
                    {
                        var tenantId = tenantGroup.Key;
                        var currentIps = tenantGroup
                            .Select(x => x.IpAddress)
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .ToHashSet(StringComparer.OrdinalIgnoreCase);

                        previousIpsByTenant.TryGetValue(tenantId, out var previousIps);
                        previousIps ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                        int returningVisitors = currentIps.Intersect(previousIps).Count();
                        int uniqueVisitors = currentIps.Count;

                        await UpdateDailyAnalytics(tenantId, date, tenantGroup.Count(), uniqueVisitors, returningVisitors, cancellationToken);
                        await UpdateDailyDevices(tenantId, date, tenantGroup, cancellationToken);
                        await UpdateDailyPages(tenantId, date, tenantGroup, cancellationToken);
                        await UpdateDailyReferrers(tenantId, date, tenantGroup, cancellationToken);
                        await UpdateHourlyAnalytics(tenantId, date, tenantGroup, cancellationToken);

                        previousIps.UnionWith(currentIps);
                        previousIpsByTenant[tenantId] = previousIps;
                    }
                }

                await _dataStore.SaveChangesAsync(cancellationToken);

            }

        }

        private async Task<DateOnly> GetLastProcessedDate(CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();

            var latestDate = await repository.FirstOrDefaultAsync(new LastAnalyticsDateSpecification(), s => s.Date, cancellationToken);

            return latestDate == default ? DateOnly.MinValue : latestDate;
        }

        private async Task UpdateDailyAnalytics(Guid tenantId, DateTime date, int pageViews, int uniqueVisitors, int returningVisitors, CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();

            var entityDate = DateOnly.FromDateTime(date);

            var entity = await repository.FirstOrDefaultAsync(new AnalyticsDailyDateEntrySpecification(entityDate, tenantId), cancellationToken);

            if (entity == null)
            {
                entity = new AnalyticsDaily(tenantId)
                {
                    Date = entityDate
                };
                repository.Create(entity);
            }

            entity.PageViews += pageViews;
            entity.UniqueVisitors += uniqueVisitors;
            entity.ReturningVisitors += returningVisitors;
        }

        private async Task UpdateDailyDevices(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup, CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyDevices>();

            var entityDate = DateOnly.FromDateTime(date);

            foreach (var deviceGroup in tenantGroup.GroupBy(x => x.Device))
            {
                var entity = await repository
                    .FirstOrDefaultAsync(new DailyDevicesForDateSpecification(tenantId, entityDate, deviceGroup.Key), cancellationToken);

                if (entity == null)
                {
                    entity = new AnalyticsDailyDevices(tenantId)
                    {
                        Date = entityDate,
                        Device = deviceGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views += deviceGroup.Count();
            }
        }

        /// <summary>
        /// THis ones problematic
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="date"></param>
        /// <param name="tenantGroup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task UpdateDailyPages(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup, CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyPages>();

            var entityDate = DateOnly.FromDateTime(date);

            foreach (var pageGroup in tenantGroup.GroupBy(x => x.Url))
            {
                var entity = await repository.FirstOrDefaultAsync(new DailyPagesForTenantSpecification(tenantId, entityDate, pageGroup.Key), cancellationToken);

                if (entity == null)
                {
                    entity = new AnalyticsDailyPages(tenantId)
                    {
                        Date = entityDate,
                        Url = pageGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views += pageGroup.Count();
            }
        }

        private async Task UpdateDailyReferrers(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup, CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyReferrers>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var referrerGroup in tenantGroup.GroupBy(x => x.Referrer))
            {
                var entity = await repository.FirstOrDefaultAsync(new DailyRererersForTenantSpecification(tenantId, entityDate, referrerGroup.Key), cancellationToken);

                if (entity == null)
                {
                    entity = new AnalyticsDailyReferrers(tenantId)
                    {
                        Date = entityDate,
                        Referrer = referrerGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views += referrerGroup.Count();
            }
        }

        private async Task UpdateHourlyAnalytics(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup, CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsHourly>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var hourlyGroup in tenantGroup.GroupBy(x => x.HourOfDay))
            {
                var entity = await repository.FirstOrDefaultAsync(new HourlyAnalyticsForTenantSpecification(tenantId, entityDate, hourlyGroup.Key), cancellationToken);

                if (entity == null)
                {
                    entity = new AnalyticsHourly(tenantId)
                    {
                        Date = entityDate,
                        HourOfDay = hourlyGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views += hourlyGroup.Count();
            }
        }

        private static string NormalizeDevice(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value.Trim();
        }

        private static string NormalizeUrl(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim().ToLower();
        }

        private static string NormalizeReferrer(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "(direct)" : value.Trim();
        }

        private sealed class PageViewAggregate
        {
            public Guid ApplicationTenantId { get; set; }
            public DateOnly Date { get; set; }
            public string IpAddress { get; set; } = string.Empty;
            public string Device { get; set; } = string.Empty;
            public string Url { get; set; } = string.Empty;
            public string Referrer { get; set; } = string.Empty;
            public int HourOfDay { get; set; }
        }
    }
}
