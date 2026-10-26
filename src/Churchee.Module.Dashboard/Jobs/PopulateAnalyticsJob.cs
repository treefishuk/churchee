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

            var pageViewStartDate = lastProcessedDate == DateOnly.MinValue
                ? DateTime.MinValue
                : lastProcessedDate.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

            var pageViewRepository = _dataStore.GetRepository<PageView>();

            const int batchSize = 1000;

            var skip = 0;
            var processedDailyAnalytics = new HashSet<(Guid ApplicationTenantId, DateOnly Date)>();
            var processedDailyDevices = new HashSet<(Guid ApplicationTenantId, DateOnly Date, string Device)>();
            var processedDailyPages = new HashSet<(Guid ApplicationTenantId, DateOnly Date, string Url)>();
            var processedDailyReferrers = new HashSet<(Guid ApplicationTenantId, DateOnly Date, string Referrer)>();
            var processedHourlyAnalytics = new HashSet<(Guid ApplicationTenantId, DateOnly Date, int HourOfDay)>();

            while (true)
            {
                var datePageViews = await pageViewRepository
                    .GetListAsync(new PageViewsAfterDateBatchSpecification(pageViewStartDate, batchSize, skip),
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
                var dailyAnalytics = new List<AnalyticsDaily>();
                var dailyDevices = new List<AnalyticsDailyDevices>();
                var dailyPages = new List<AnalyticsDailyPages>();
                var dailyReferrers = new List<AnalyticsDailyReferrers>();
                var hourlyAnalytics = new List<AnalyticsHourly>();

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

                        var entityDate = DateOnly.FromDateTime(date);
                        var dailyAnalyticsKey = (tenantId, entityDate);
                        if (!processedDailyAnalytics.Add(dailyAnalyticsKey))
                        {
                            continue;
                        }

                        dailyAnalytics.Add(new AnalyticsDaily(tenantId)
                        {
                            Date = entityDate,
                            PageViews = tenantGroup.Count(),
                            UniqueVisitors = uniqueVisitors,
                            ReturningVisitors = returningVisitors
                        });

                        foreach (var deviceGroup in tenantGroup.GroupBy(x => x.Device))
                        {
                            var deviceKey = (tenantId, entityDate, deviceGroup.Key);
                            if (!processedDailyDevices.Add(deviceKey))
                            {
                                continue;
                            }

                            dailyDevices.Add(new AnalyticsDailyDevices(tenantId)
                            {
                                Date = entityDate,
                                Device = deviceGroup.Key,
                                Views = deviceGroup.Count()
                            });
                        }

                        foreach (var pageGroup in tenantGroup.GroupBy(x => x.Url))
                        {
                            var pageKey = (tenantId, entityDate, pageGroup.Key);
                            if (!processedDailyPages.Add(pageKey))
                            {
                                continue;
                            }

                            dailyPages.Add(new AnalyticsDailyPages(tenantId)
                            {
                                Date = entityDate,
                                Url = pageGroup.Key,
                                Views = pageGroup.Count()
                            });
                        }

                        foreach (var referrerGroup in tenantGroup.GroupBy(x => x.Referrer))
                        {
                            var referrerKey = (tenantId, entityDate, referrerGroup.Key);
                            if (!processedDailyReferrers.Add(referrerKey))
                            {
                                continue;
                            }

                            dailyReferrers.Add(new AnalyticsDailyReferrers(tenantId)
                            {
                                Date = entityDate,
                                Referrer = referrerGroup.Key,
                                Views = referrerGroup.Count()
                            });
                        }

                        foreach (var hourlyGroup in tenantGroup.GroupBy(x => x.HourOfDay))
                        {
                            var hourlyKey = (tenantId, entityDate, hourlyGroup.Key);
                            if (!processedHourlyAnalytics.Add(hourlyKey))
                            {
                                continue;
                            }

                            hourlyAnalytics.Add(new AnalyticsHourly(tenantId)
                            {
                                Date = entityDate,
                                HourOfDay = hourlyGroup.Key,
                                Views = hourlyGroup.Count()
                            });
                        }

                        previousIps.UnionWith(currentIps);
                        previousIpsByTenant[tenantId] = previousIps;
                    }
                }

                await BulkInsertOrUpdateAnalyticsAsync(dailyAnalytics, dailyDevices, dailyPages, dailyReferrers, hourlyAnalytics, cancellationToken);

            }

        }

        private async Task<DateOnly> GetLastProcessedDate(CancellationToken cancellationToken)
        {
            var repository = _dataStore.GetRepository<AnalyticsDaily>();

            var latestDate = await repository.FirstOrDefaultAsync(new LastAnalyticsDateSpecification(), s => s.Date, cancellationToken);

            return latestDate == default ? DateOnly.MinValue : latestDate;
        }

        private async Task BulkInsertOrUpdateAnalyticsAsync(
            IList<AnalyticsDaily> dailyAnalytics,
            IList<AnalyticsDailyDevices> dailyDevices,
            IList<AnalyticsDailyPages> dailyPages,
            IList<AnalyticsDailyReferrers> dailyReferrers,
            IList<AnalyticsHourly> hourlyAnalytics,
            CancellationToken cancellationToken)
        {
            if (dailyAnalytics.Count > 0)
            {
                await _dataStore.GetRepository<AnalyticsDaily>().BulkInsertOrUpdateAsync(dailyAnalytics, cancellationToken);
            }

            if (dailyDevices.Count > 0)
            {
                await _dataStore.GetRepository<AnalyticsDailyDevices>().BulkInsertOrUpdateAsync(dailyDevices, cancellationToken);
            }

            if (dailyPages.Count > 0)
            {
                await _dataStore.GetRepository<AnalyticsDailyPages>().BulkInsertOrUpdateAsync(dailyPages, cancellationToken);
            }

            if (dailyReferrers.Count > 0)
            {
                await _dataStore.GetRepository<AnalyticsDailyReferrers>().BulkInsertOrUpdateAsync(dailyReferrers, cancellationToken);
            }

            if (hourlyAnalytics.Count > 0)
            {
                await _dataStore.GetRepository<AnalyticsHourly>().BulkInsertOrUpdateAsync(hourlyAnalytics, cancellationToken);
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
