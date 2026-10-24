using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;

using Churchee.Module.Dashboard.Entities;

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
            var lastProcessedDate = GetLastProcessedDate();

            var pageViewRepository = _dataStore.GetRepository<PageView>();

            var dates = pageViewRepository
                .GetQueryable()
                .Where(x => !x.Deleted && x.ViewedAt >= lastProcessedDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
                .Select(x => x.ViewedAt.Date)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            if (dates.Count == 0)
            {
                return;
            }

            var previousIpsByTenant = new Dictionary<Guid, HashSet<string>>();

            foreach (var date in dates)
            {
                var datePageViews = pageViewRepository
                    .GetQueryable()
                    .Where(x => !x.Deleted && x.ViewedAt.Date == date)
                    .Select(x => new PageViewAggregate
                    {
                        ApplicationTenantId = x.ApplicationTenantId,
                        Date = DateOnly.FromDateTime(x.ViewedAt.Date),
                        IpAddress = string.IsNullOrWhiteSpace(x.IpAddress) ? string.Empty : x.IpAddress.Trim(),
                        Device = NormalizeDevice(x.Device),
                        Url = NormalizeUrl(x.Url),
                        Referrer = NormalizeReferrer(x.Referrer),
                        HourOfDay = x.ViewedAt.Hour
                    })
                    .ToList();

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

                    UpdateDailyAnalytics(tenantId, date, tenantGroup.Count(), uniqueVisitors, returningVisitors);
                    UpdateDailyDevices(tenantId, date, tenantGroup);
                    UpdateDailyPages(tenantId, date, tenantGroup);
                    UpdateDailyReferrers(tenantId, date, tenantGroup);
                    UpdateHourlyAnalytics(tenantId, date, tenantGroup);

                    previousIps.UnionWith(currentIps);
                    previousIpsByTenant[tenantId] = previousIps;
                }
            }

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private DateOnly GetLastProcessedDate()
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();
            var latestDate = repository.GetQueryable()
                .Where(x => !x.Deleted)
                .OrderByDescending(x => x.Date)
                .Select(x => x.Date)
                .FirstOrDefault();

            return latestDate == default ? DateOnly.MinValue : latestDate;
        }

        private void UpdateDailyAnalytics(Guid tenantId, DateTime date, int pageViews, int uniqueVisitors, int returningVisitors)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();
            var entityDate = DateOnly.FromDateTime(date);

            var entity = repository.GetQueryable()
                .FirstOrDefault(x => x.ApplicationTenantId == tenantId && x.Date == entityDate);

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

        private void UpdateDailyDevices(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
        {
            var repository = _dataStore.GetRepository<AnalyticsDailyDevices>();
            var entityDate = DateOnly.FromDateTime(date);

            foreach (var deviceGroup in tenantGroup.GroupBy(x => x.Device))
            {
                var entity = repository.GetQueryable()
                    .FirstOrDefault(x => x.ApplicationTenantId == tenantId && x.Date == entityDate && x.Device == deviceGroup.Key);

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

        private void UpdateDailyPages(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
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

        private void UpdateDailyReferrers(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
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

        private void UpdateHourlyAnalytics(Guid tenantId, DateTime date, IGrouping<Guid, PageViewAggregate> tenantGroup)
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

        private static string NormalizeDevice(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "Unknown" : value.Trim();
        }

        private static string NormalizeUrl(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? "/" : value.Trim();
        }

        private static string NormalizeReferrer(string? value)
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
