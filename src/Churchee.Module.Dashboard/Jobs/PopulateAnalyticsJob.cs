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

            var dates = await pageViewRepository
                .GetListAsync(new PageViewsAfterDateSpecification(lastProcessedDateTime),
                s => s.ViewedAt.Date,
                cancellationToken);

            if (dates.Count == 0)
            {
                return;
            }

            var previousIpsByTenant = new Dictionary<Guid, HashSet<string>>();

            foreach (var date in dates)
            {
                var datePageViews = await pageViewRepository
                    .GetListAsync(new PageViewsAfterDateSpecification(lastProcessedDateTime),
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
                    continue;
                }

                foreach (var tenantGroup in datePageViews.GroupBy(x => x.ApplicationTenantId))
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

                    await UpdateDailyAnalytics(tenantId, date, tenantGroup.Count(), uniqueVisitors, returningVisitors);
                    await UpdateDailyDevices(tenantId, date, tenantGroup);
                    await UpdateDailyPages(tenantId, date, tenantGroup);
                    await UpdateDailyReferrers(tenantId, date, tenantGroup);
                    await UpdateHourlyAnalytics(tenantId, date, tenantGroup);

                    previousIps.UnionWith(currentIps);
                    previousIpsByTenant[tenantId] = previousIps;
                }
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private async Task<DateOnly> GetLastProcessedDate(CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();

            var latestDate = await repository.FirstOrDefaultAsync(new LastAnalyticsDateSpecification(), s => s.Date, cancellationToken);

            return latestDate == default ? DateOnly.MinValue : latestDate;
        }

        private async Task UpdateDailyAnalytics(Guid tenantId, DateTime date, int pageViews, int uniqueVisitors, int returningVisitors)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();

            var entityDate = DateOnly.FromDateTime(date);

            var entity = await repository.FirstOrDefaultAsync(new AnalyticsDailyDateEntrySpecification(entityDate, tenantId), CancellationToken.None);

            if (entity == null)
            {
                entity = new AnalyticsDaily(tenantId)
                {
                    Date = entityDate
                };
                repository.Create(entity);
            }

            entity.PageViews = pageViews;
            entity.UniqueVisitors = uniqueVisitors;
            entity.ReturningVisitors = returningVisitors;
        }

        private async Task UpdateDailyDevices(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyDevices>();

            var entityDate = DateOnly.FromDateTime(date);

            foreach (var deviceGroup in tenantGroup.GroupBy(x => x.Device))
            {
                var entity = await repository
                    .FirstOrDefaultAsync(new DailyDevicesForDateSpecification(tenantId, entityDate, deviceGroup.Key), CancellationToken.None);

                if (entity == null)
                {
                    entity = new AnalyticsDailyDevices(tenantId)
                    {
                        Date = entityDate,
                        Device = deviceGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views = deviceGroup.Count();
            }
        }

        private async Task UpdateDailyPages(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyPages>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var pageGroup in tenantGroup.GroupBy(x => x.Url))
            {
                var entity = repository.GetQueryable()
                    .FirstOrDefault(x => x.ApplicationTenantId == tenantId && x.Date == entityDate && x.Url == pageGroup.Key);

                if (entity == null)
                {
                    entity = new AnalyticsDailyPages(tenantId)
                    {
                        Date = entityDate,
                        Url = pageGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views = pageGroup.Count();
            }
        }

        private async Task UpdateDailyReferrers(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyReferrers>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var referrerGroup in tenantGroup.GroupBy(x => x.Referrer))
            {
                var entity = repository.GetQueryable()
                    .FirstOrDefault(x => x.ApplicationTenantId == tenantId && x.Date == entityDate && x.Referrer == referrerGroup.Key);

                if (entity == null)
                {
                    entity = new AnalyticsDailyReferrers(tenantId)
                    {
                        Date = entityDate,
                        Referrer = referrerGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views = referrerGroup.Count();
            }
        }

        private async Task UpdateHourlyAnalytics(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
        {
            var repository = _dataStore.GetRepository<AnalyticsHourly>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var hourlyGroup in tenantGroup.GroupBy(x => x.HourOfDay))
            {
                var entity = repository.GetQueryable()
                    .FirstOrDefault(x => x.ApplicationTenantId == tenantId && x.Date == entityDate && x.HourOfDay == hourlyGroup.Key);

                if (entity == null)
                {
                    entity = new AnalyticsHourly(tenantId)
                    {
                        Date = entityDate,
                        HourOfDay = hourlyGroup.Key
                    };
                    repository.Create(entity);
                }

                entity.Views = hourlyGroup.Count();
            }
        }

        private static string NormalizeDevice(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value.Trim();
        }

        private static string NormalizeUrl(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
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
