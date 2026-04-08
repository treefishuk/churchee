using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Helpers;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.ChurchSuite.API;
using Churchee.Module.ChurchSuite.Events.Helpers;
using Churchee.Module.ChurchSuite.Events.Specifications;
using Churchee.Module.ChurchSuite.Helpers;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Churchee.Module.ChurchSuite.Jobs
{
    public partial class SyncChurchSuiteEventsJob : IJob
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
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Converters = { new ChurchSuiteDateTimeConverter() }
            };
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

        }

        private async Task ProcessGrouping(Guid applicationTenantId, IEnumerable<IGrouping<Grouping, ApiResponse>> grouped, string parentSlug, Guid? parentId, Guid pageTypeId, Common.Abstractions.Storage.IRepository<Event> repo, CancellationToken cancellationToken)
        {
            foreach (var item in grouped)
            {

                string sourceId = (item.Key.Sequence ?? 0).ToString();

                var dbEvent = await repo.FirstOrDefaultAsync(new GetEventByChurchSuiteSequenceSpecification(sourceId), cancellationToken);

                if (dbEvent == null)
                {
                    await CreateNewEvent(applicationTenantId, parentSlug, parentId, pageTypeId, item, sourceId, cancellationToken);
                }
                else
                {
                    await UpdateEvent(item, dbEvent, applicationTenantId, cancellationToken);
                }

            }
        }

        private async Task UpdateEvent(IGrouping<Grouping, ApiResponse> item, Event dbEvent, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            dbEvent.UpdateInformation(item.Key.Name ?? dbEvent.Title, dbEvent.Description, item.Key.Description ?? dbEvent.Description);
            dbEvent.UpdateLocation(item.Key.LocationName ?? dbEvent.LocationName, null, item.Key.LocationAddress ?? dbEvent.Street, string.Empty, string.Empty, item.Key.LocationLatitude, item.Key.LocationLongitude);

            string imageUrl = item.Key.ImageLargeUrl;

            await ConvertImageToLocalImage(dbEvent, imageUrl, applicationTenantId, cancellationToken);

            var churchSuiteDates = item.Select(s => new { Start = s.DatetimeStart, End = s.DatetimeEnd, BookingUrl = s.SignupOptions.SignupEnabled == "1" ? s.SignupOptions.Tickets.Url : null }).ToList();

            var datesToAdd = churchSuiteDates.Where(ed => !dbEvent.EventDates.Any(a => a.Start.Value.Date != ed.Start.Date)).ToList();

            var datesToRemove = dbEvent.EventDates.Where(ed => !churchSuiteDates.Any(a => a.Start.Date == ed.Start.Value.Date)).ToList();

            if (datesToAdd.Count == 0 && datesToRemove.Count == 0)
            {
                return;
            }

            foreach (var date in datesToRemove)
            {
                dbEvent.EventDates.Remove(date);
            }

            foreach (var date in datesToAdd)
            {
                dbEvent.AddDate(Guid.NewGuid(), date.Start, date.End, date.BookingUrl);
            }

            try
            {
                await _dataStore.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update event {Event}", item.Key.Sequence);
            }

        }

        private async Task CreateNewEvent(Guid applicationTenantId, string parentSlug, Guid? parentId, Guid pageTypeId, IGrouping<Grouping, ApiResponse> item, string sourceId, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Event>();

            var newEvent = new Event.Builder()
                 .SetApplicationTenantId(applicationTenantId)
                 .SetParentId(parentId)
                 .SetParentSlug(parentSlug)
                 .SetPageTypeId(pageTypeId)
                 .SetSourceName("ChurchSuite")
                 .SetSourceId(sourceId)
                 .SetTitle(item.Key.Name)
                 .SetDescription(item.Key.Name + " at " + item.Key.LocationAddress)
                 .SetContent(item.Key.Description)
                 .SetLocationName(item.Key.LocationName)
                 .SetLatitude(item.Key.LocationLatitude == null ? null : Convert.ToDecimal(item.Key.LocationLatitude.Value))
                 .SetLongitude(item.Key.LocationLongitude == null ? null : Convert.ToDecimal(item.Key.LocationLongitude.Value))
                 .SetImageUrl(item.Key.ImageLargeUrl)
                 .SetPublished(true)
                 .Build();

            foreach (var date in item)
            {
                newEvent.AddDate(Guid.NewGuid(), date.DatetimeStart, date.DatetimeEnd);
            }

            await ConvertImageToLocalImage(newEvent, newEvent.ImageUrl, applicationTenantId, cancellationToken);

            SuffixGeneration.AddUniqueSuffixIfNeeded(newEvent, _dataStore.GetRepository<Event>());

            repo.Create(newEvent);

            try
            {
                await _dataStore.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event {Event}", item.Key.Sequence);
            }

        }

        internal async Task<IEnumerable<IGrouping<Grouping, ApiResponse>>> GetGroupedData(Guid applicationTenantId)
        {
            var result = await GetFeedResult(applicationTenantId);

            var activeEvents = result.Where(x => x.Status == "confirmed");

            var grouped = activeEvents.GroupBy(x => new Grouping
            {
                Sequence = x.Sequence,
                Name = x.Name,
                Description = x.Description,
                LocationAddress = x.Location.Address,
                LocationLatitude = x.Location.Latitude == null ? (decimal?)null : Convert.ToDecimal(x.Location.Latitude.Value),
                LocationLongitude = x.Location.Longitude == null ? (decimal?)null : Convert.ToDecimal(x.Location.Longitude.Value),
                ImageSmallUrl = x.Images.Small.Url,
                ImageMediumUrl = x.Images.Medium.Url,
                ImageLargeUrl = x.Images.Large.Url
            });

            return grouped;
        }

        internal async Task<IEnumerable<ApiResponse>> GetFeedResult(Guid tenantId)
        {
            var client = _clientFactory.CreateClient();

            string churchSuiteUri = await _settingStore.GetSettingValue(Guid.Parse(SettingKeys.ChurchSuiteEventsUrl), tenantId);

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

                string friendlyFileName = $"{churchSuiteEvent.Title.ToURL()}.webp";

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
    }
}
