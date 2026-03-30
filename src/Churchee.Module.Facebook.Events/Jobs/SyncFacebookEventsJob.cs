using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Facebook.Events.API;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Facebook.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Churchee.Module.Facebook.Events.Jobs
{
    public class SyncFacebookEventsJob : IJob
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly IBlobStore _blobStore;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IJobService _jobShedularService;
        private readonly IImageProcessor _imageProcessor;
        private readonly ILogger<SyncFacebookEventsJob> _logger;

        public SyncFacebookEventsJob(IHttpClientFactory clientFactory, ISettingStore settingStore, IDataStore dataStore, IBlobStore blobStore, IJobService jobShedularService, ILogger<SyncFacebookEventsJob> logger, IImageProcessor imageProcessor)
        {
            _clientFactory = clientFactory;
            _settingStore = settingStore;
            _dataStore = dataStore;
            _blobStore = blobStore;
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new DateTimeIso8601JsonConverter());
            _jobShedularService = jobShedularService;
            _logger = logger;
            _imageProcessor = imageProcessor;
        }

        public async Task ExecuteAsync(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            try
            {
                var client = _clientFactory.CreateClient("Facebook");

                var tokenRepo = _dataStore.GetRepository<Token>();

                string facebookPageAccessToken = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.FacebookPageAccessToken, applicationTenantId), s => s.Value, cancellationToken);

                string pageId = await _settingStore.GetSettingValue(Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68"), applicationTenantId);

                var feedResponseItems = await GetFeedResult(client, pageId, facebookPageAccessToken);

                if (!feedResponseItems.Any())
                {
                    return;
                }

                var eventIds = feedResponseItems.Where(w => !string.IsNullOrEmpty(w.Story) && (w.Story.Contains("created an event") || w.Story.Contains("added an event"))).Select(s => s.Id.Replace($"{pageId}_", "")).ToList();

                if (eventIds.Count == 0)
                {
                    return;
                }

                var repo = _dataStore.GetRepository<Event>();

                var pageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

                foreach (string eventId in eventIds)
                {
                    var dbPost = await repo.FirstOrDefaultAsync(new GetEventByFacebookIdSpecification(eventId), cancellationToken);

                    if (dbPost != null)
                    {
                        await UpdateEvent(applicationTenantId, dbPost, client, facebookPageAccessToken, eventId, cancellationToken);
                    }
                    else
                    {
                        await CreateNewEvent(applicationTenantId, client, facebookPageAccessToken, repo, pageTypeId, eventId, cancellationToken);
                    }
                }

                await _dataStore.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error Syncing Facebook Events");
            }
        }

        private async Task<IEnumerable<FacebookFeedResponseItem>> GetFeedResult(HttpClient client, string pageId, string accessToken)
        {
            string feedJsonString = await client.GetStringAsync($"{pageId}/feed?access_token={accessToken}&limit=100");

            return string.IsNullOrEmpty(feedJsonString)
                ? []
                : (IEnumerable<FacebookFeedResponseItem>)JsonSerializer.Deserialize<FacebookFeedResponse>(feedJsonString, _jsonSerializerOptions).Data;
        }

        private async Task UpdateEvent(Guid applicationTenantId, Event dbEvent, HttpClient client, string facebookPageAccessToken, string eventId, CancellationToken cancellationToken)
        {
            string facebookEventJson = await client.GetStringAsync($"{eventId}?access_token={facebookPageAccessToken}&format=json&fields=cover,description,name,place,start_time,end_time", cancellationToken);

            var item = JsonSerializer.Deserialize<FacebookEventResult>(facebookEventJson, _jsonSerializerOptions);

            if (item == null)
            {
                return;
            }

            dbEvent.UpdateInformation(item.Name ?? dbEvent.Title, item.Description ?? dbEvent.Description, item.Description ?? dbEvent.Description);
            dbEvent.UpdateLocation(item.Place?.Name ?? dbEvent.LocationName, item.Place?.Location?.City ?? dbEvent.City, item.Place?.Location?.Street ?? dbEvent.Street, item.Place?.Location?.Zip ?? dbEvent.PostCode, item.Place?.Location?.Country ?? dbEvent.Country, Convert.ToDecimal(item.Place?.Location?.Latitude ?? 0d), Convert.ToDecimal(item.Place?.Location?.Longitude ?? 0d));

            string imageUrl = item.Cover?.Source ?? string.Empty;

            await ConvertImageToLocalImage(dbEvent, imageUrl, applicationTenantId, cancellationToken);

            await UpdateEventDateTime(item, dbEvent.Id, applicationTenantId, cancellationToken);
        }

        private async Task CreateNewEvent(Guid applicationTenantId, HttpClient client, string facebookPageAccessToken, IRepository<Event> repo, Guid pageTypeId, string eventId, CancellationToken cancellationToken)
        {
            string facebookEventJson = await client.GetStringAsync($"{eventId}?access_token={facebookPageAccessToken}&format=json&fields=cover,description,name,place,start_time,end_time", cancellationToken);

            var item = JsonSerializer.Deserialize<FacebookEventResult>(facebookEventJson, _jsonSerializerOptions);

            if (item == null)
            {
                return;
            }

            string parentSlug = "/events";

            Guid? parentId = null;

            var parentPage = await _dataStore.GetRepository<Page>().FirstOrDefaultAsync(new EventListingPageSpecification(), cancellationToken);

            if (parentPage != null)
            {
                parentSlug = parentPage.Url;
                parentId = parentPage.Id;
            }

            var newEvent = new Event.Builder()
                .SetApplicationTenantId(applicationTenantId)
                .SetParentId(parentId)
                .SetParentSlug(parentSlug)
                .SetPageTypeId(pageTypeId)
                .SetSourceName("Facebook")
                .SetSourceId(item.Id)
                .SetTitle(item.Name ?? string.Empty)
                .SetDescription(item.Description)
                .SetContent(item.Description)
                .SetLocationName(item.Place?.Name ?? string.Empty)
                .SetCity(item.Place?.Location?.City ?? string.Empty)
                .SetStreet(item.Place?.Location?.Street ?? string.Empty)
                .SetPostCode(item.Place?.Location?.Zip ?? string.Empty)
                .SetCountry(item.Place?.Location?.Country ?? string.Empty)
                .SetLatitude(Convert.ToDecimal(item.Place?.Location?.Latitude ?? 0d))
                .SetLongitude(Convert.ToDecimal(item.Place?.Location?.Longitude ?? 0d))
                .SetDates(item.StartTime, item.EndTime)
                .Build();

            string imageUrl = item.Cover?.Source ?? string.Empty;

            await ConvertImageToLocalImage(newEvent, imageUrl, applicationTenantId, cancellationToken);

            SuffixGeneration.AddUniqueSuffixIfNeeded(newEvent, _dataStore.GetRepository<Event>());

            repo.Create(newEvent);
        }

        internal async Task UpdateEventDateTime(FacebookEventResult facebookEventResult, Guid eventId, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string timeZoneSetting = await _settingStore.GetSettingValue(Guid.Parse("1a1d575c-40ed-4ce8-b7f0-4fcd176be0d9"), applicationTenantId);

            var timezone = TimeZoneInfo.Utc;

            if (!string.IsNullOrEmpty(timeZoneSetting))
            {
                timezone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneSetting);
            }

            var eventDateRepo = _dataStore.GetRepository<EventDate>();

            var date = await eventDateRepo.FirstOrDefaultAsync(new EventDatesForEventSpecification(eventId), cancellationToken);

            var startTime = TimeZoneInfo.ConvertTimeFromUtc(facebookEventResult.StartTime ?? DateTime.UtcNow, timezone);

            if (date.Start == startTime && facebookEventResult.EndTime == null)
            {
                return;
            }

            if (facebookEventResult.EndTime != null)
            {
                var endTime = TimeZoneInfo.ConvertTimeFromUtc(facebookEventResult.EndTime ?? DateTime.UtcNow, timezone);
                date.End = endTime;
            }

            date.Start = startTime;

        }

        private async Task ConvertImageToLocalImage(Event facebookEvent, string facebookImageUrl, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(facebookImageUrl))
            {
                return;
            }

            var response = await _clientFactory.CreateClient().GetAsync(facebookImageUrl, cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var tempFileStream = await response.Content.ReadAsStreamAsync(cancellationToken);

            string hash = await Hasher.HashFirst64KbAsync(tempFileStream, cancellationToken);

            if (facebookEvent.ImageCheckHash == hash)
            {
                return;
            }

            try
            {
                using var webPStream = await _imageProcessor.ConvertToWebP(tempFileStream, cancellationToken);

                string friendlyFileName = $"{facebookEvent.Title.Replace(" ", "_")}.webp";

                string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, $"/img/events/{friendlyFileName}", webPStream, true, cancellationToken);

                facebookEvent.SetImageUrl($"/img/events/{friendlyFileName}");
                facebookEvent.SetImageCheckHash(hash);

                _jobShedularService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalImagePath, true, CancellationToken.None));
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Error processing Facebook Event image for event {EventId}", facebookEvent.Id);
                }
            }
        }
    }
}
