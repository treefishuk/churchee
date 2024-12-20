using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Events.Specifications;
using Churchee.Module.Facebook.Events.API;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Facebook.Events.Specifications;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Event = Churchee.Module.Events.Entities.Event;

namespace Churchee.Module.Facebook.Events.Features.Commands.SyncFacebookEventsToEventsTable
{
    public class SyncFacebookEventsToEventsTableCommandHandler : IRequestHandler<SyncFacebookEventsToEventsTableCommand, CommandResponse>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly IBlobStore _blobStore;
        private readonly ICurrentUser _currentUser;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly IJobService _jobShedularService;
        private readonly ILogger<SyncFacebookEventsToEventsTableCommandHandler> _logger;

        public SyncFacebookEventsToEventsTableCommandHandler(IHttpClientFactory clientFactory, ISettingStore settingStore, IDataStore dataStore, IBlobStore blobStore, ICurrentUser currentUser, ILogger<SyncFacebookEventsToEventsTableCommandHandler> logger, IJobService jobShedularService)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _settingStore = settingStore ?? throw new ArgumentNullException(nameof(settingStore));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _blobStore = blobStore ?? throw new ArgumentNullException(nameof(blobStore));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerializerOptions = new JsonSerializerOptions();
            _jsonSerializerOptions.Converters.Add(new DateTimeIso8601JsonConverter());
            _jobShedularService = jobShedularService;
        }

        public async Task<CommandResponse> Handle(SyncFacebookEventsToEventsTableCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                _jobShedularService.SheduleJob($"{applicationTenantId}_FacebookEvents", () => SyncFacebookEvents(applicationTenantId, cancellationToken), Cron.Daily);

                _jobShedularService.QueueJob(() => SyncFacebookEvents(applicationTenantId, cancellationToken));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing Facebook events");

                response.AddError("Failed To Sync", "");
            }

            return response;

        }

        public async Task SyncFacebookEvents(Guid applicationTenantId, CancellationToken cancellationToken)
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

            Guid pageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            foreach (string eventId in eventIds)
            {
                var dbPost = await repo.ApplySpecification(new GetEventByFacebookIdSpecification(eventId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (dbPost != null)
                {
                    continue;
                }

                await CreateNewEvent(applicationTenantId, client, facebookPageAccessToken, repo, pageTypeId, eventId, cancellationToken);

            }

            await _dataStore.SaveChangesAsync(cancellationToken);

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
                .SetImageUrl(item.Cover?.Source ?? string.Empty)
                .Build();

            await ConvertImageToLocalImage(newEvent, applicationTenantId, cancellationToken);

            repo.Create(newEvent);
        }

        private async Task<IEnumerable<FacebookFeedResponseItem>> GetFeedResult(HttpClient client, string pageId, string accessToken)
        {
            string feedJsonString = await client.GetStringAsync($"{pageId}/feed?access_token={accessToken}&limit=100");

            if (string.IsNullOrEmpty(feedJsonString))
            {
                return [];
            }

            return JsonSerializer.Deserialize<FacebookFeedResponse>(feedJsonString, _jsonSerializerOptions).Data;
        }

        private async Task ConvertImageToLocalImage(Event facebookEvent, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string fileName = Path.GetFileName(facebookEvent.ImageUrl.Split('?')[0]);

            string fileExt = Path.GetExtension(fileName);

            string friendlyFileName = $"{facebookEvent.Title.Replace(" ", "_")}{fileExt}";

            var response = await _clientFactory.CreateClient().GetAsync($"{facebookEvent.ImageUrl}", cancellationToken);

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

            string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, $"/img/events/{friendlyFileName}", stream, true, cancellationToken);

            facebookEvent.SetImageUrl($"/img/events/{friendlyFileName}");

            var bytes = stream.ConvertStreamToByteArray();

            _jobShedularService.QueueJob<ImageCropsGenerator>(x => x.CreateCrops(applicationTenantId, finalImagePath, bytes, true));
        }
    }
}
