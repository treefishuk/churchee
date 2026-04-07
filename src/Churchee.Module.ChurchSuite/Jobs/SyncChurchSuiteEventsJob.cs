using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Helpers;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.ChurchSuite.API;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text.Json;

namespace Churchee.Module.ChurchSuite.Jobs
{
    public class SyncChurchSuiteEventsJob : IJob
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly IBlobStore _blobStore;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IJobService _jobShedularService;
        private readonly IImageProcessor _imageProcessor;
        private readonly ILogger _logger;

        public SyncChurchSuiteEventsJob(IHttpClientFactory clientFactory, ISettingStore settingStore, IDataStore dataStore, IBlobStore blobStore, IJobService jobShedularService, IImageProcessor imageProcessor, ILogger<SyncChurchSuiteEventsJob> logger)
        {
            _clientFactory = clientFactory;
            _settingStore = settingStore;
            _dataStore = dataStore;
            _blobStore = blobStore;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jobShedularService = jobShedularService;
            _imageProcessor = imageProcessor;
            _logger = logger;
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var grouped = await GetGroupedData(applicationTenantId);

            string parentSlug = "/events";

            Guid? parentId = null;

            var parentPage = await _dataStore.GetRepository<Page>().FirstOrDefaultAsync(new EventListingPageSpecification(), cancellationToken);

            if (parentPage != null)
            {
                parentSlug = parentPage.Url;
                parentId = parentPage.Id;
            }

            var pageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            var repo = _dataStore.GetRepository<Event>();

            await ProcessGrouping(applicationTenantId, grouped, parentSlug, parentId, pageTypeId, repo, cancellationToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

        }

        private async Task ProcessGrouping(Guid applicationTenantId, IEnumerable<IGrouping<Grouping, ApiResponse>> grouped, string parentSlug, Guid? parentId, Guid pageTypeId, Common.Abstractions.Storage.IRepository<Event> repo, CancellationToken cancellationToken)
        {
            foreach (var item in grouped)
            {
                var newEvent = new Event.Builder()
                     .SetApplicationTenantId(applicationTenantId)
                     .SetParentId(parentId)
                     .SetParentSlug(parentSlug)
                     .SetPageTypeId(pageTypeId)
                     .SetSourceName("ChurchSuite")
                     .SetSourceId("N/A")
                     .SetTitle(item.Key.Name)
                     .SetDescription(item.Key.Name + " at " + item.Key.Location.Address)
                     .SetContent(item.Key.Description)
                     .SetLocationName(item.Key.Location.Address)
                     .SetLatitude(item.Key.Location.Latitude == null ? null : Convert.ToDecimal(item.Key.Location.Latitude.Value))
                     .SetLongitude(item.Key.Location.Longitude == null ? null : Convert.ToDecimal(item.Key.Location.Longitude.Value))
                     .SetImageUrl(item.Key.Images.Large.Url)
                     .SetPublished(true)
                     .Build();

                foreach (var date in item.Where(w => !string.IsNullOrEmpty(w.DatetimeStart)))
                {
                    var start = DateTime.ParseExact(date.DatetimeStart, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    if (date.DatetimeEnd == null)
                    {
                        newEvent.AddDate(Guid.NewGuid(), start, null);
                        continue;
                    }

                    var end = DateTime.ParseExact(date.DatetimeEnd, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    newEvent.AddDate(Guid.NewGuid(), start, end);
                }

                await ConvertImageToLocalImage(newEvent, newEvent.ImageUrl, applicationTenantId, cancellationToken);

                SuffixGeneration.AddUniqueSuffixIfNeeded(newEvent, _dataStore.GetRepository<Event>());

                repo.Create(newEvent);
            }
        }

        private async Task<IEnumerable<IGrouping<Grouping, ApiResponse>>> GetGroupedData(Guid applicationTenantId)
        {
            var result = await GetFeedResult(applicationTenantId);

            var activeEvents = result.Where(x => x.Status == "confirmed");

            var grouped = activeEvents.GroupBy(x => new Grouping
            {
                Sequence = x.Sequence,
                Name = x.Name,
                Description = x.Description,
                Location = x.Location//,
                //Images = x.Images
            });

            return grouped;
        }

        internal async Task<IEnumerable<ApiResponse>> GetFeedResult(Guid tenantId)
        {
            var client = _clientFactory.CreateClient();

            string churchSuiteUri = await _settingStore.GetSettingValue(Guid.Parse("9d15a41a-3f12-4907-ba9e-495f11d254dc"), tenantId);

            string feedJsonString = await client.GetStringAsync(churchSuiteUri);

            return string.IsNullOrEmpty(feedJsonString)
                ? []
                : JsonSerializer.Deserialize<List<ApiResponse>>(feedJsonString, _jsonSerializerOptions);
        }

        private async Task ConvertImageToLocalImage(Event churchSuiteEvent, string churchSuiteImageUrl, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(churchSuiteImageUrl))
            {
                return;
            }

            var response = await _clientFactory.CreateClient().GetAsync(churchSuiteImageUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var tempFileStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            string hash = await Hasher.HashFirst64KbAsync(tempFileStream, cancellationToken);

            if (churchSuiteEvent.ImageCheckHash == hash)
            {
                return;
            }

            try
            {
                using var webPStream = await _imageProcessor.ConvertToWebP(tempFileStream, cancellationToken);

                string friendlyFileName = $"{churchSuiteEvent.Title.Replace(" ", "_")}.webp";

                string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, $"/img/events/{friendlyFileName}", webPStream, true, cancellationToken);

                churchSuiteEvent.SetImageUrl($"/img/events/{friendlyFileName}");

                churchSuiteEvent.SetImageCheckHash(hash);

                _jobShedularService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalImagePath, true, CancellationToken.None));
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Error processing churchSuite Event image for event {EventId}", churchSuiteEvent.Id);
                }
            }
        }

        private class Grouping
        {
            public int? Sequence { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public Location Location { get; set; }
            public Images Images { get; set; }
        }
    }
}
