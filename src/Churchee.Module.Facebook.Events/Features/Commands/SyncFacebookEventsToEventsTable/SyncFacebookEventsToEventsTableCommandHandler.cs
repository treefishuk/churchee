using Churchee.Common.Abstractions.Auth;
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
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public SyncFacebookEventsToEventsTableCommandHandler(IHttpClientFactory clientFactory, ISettingStore settingStore, IDataStore dataStore, IBlobStore blobStore, ICurrentUser currentUser, IRecurringJobManager recurringJobManager, IBackgroundJobClient backgroundJobClient)
        {
            _clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            _settingStore = settingStore ?? throw new ArgumentNullException(nameof(settingStore));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _blobStore = blobStore ?? throw new ArgumentNullException(nameof(blobStore));
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));

            _jsonSerializerOptions = new JsonSerializerOptions();

            _jsonSerializerOptions.Converters.Add(new DateTimeISO8601JsonConverter());
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(SyncFacebookEventsToEventsTableCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                _recurringJobManager.AddOrUpdate($"{applicationTenantId}_FacebookEvents", () => SyncFacebookEvents(applicationTenantId, cancellationToken), Cron.Daily);

                _backgroundJobClient.Enqueue(() => SyncFacebookEvents(applicationTenantId, cancellationToken));
            }
            catch (Exception ex)
            {
                response.AddError("Failed To Sync", "");
            }

            return response;

        }

        public async Task SyncFacebookEvents(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri("https://graph.facebook.com/v18.0/");

            var tokenRepo = _dataStore.GetRepository<Token>();

            string facebookPageAccessToken = tokenRepo.ApplySpecification(new GetTokenByKeySpecification(SettingKeys.FacebookPageAccessToken, applicationTenantId)).Select(s => s.Value).FirstOrDefault();

            string facebookAccessToken = tokenRepo.ApplySpecification(new GetTokenByKeySpecification(SettingKeys.FacebookAccessToken, applicationTenantId)).Select(s => s.Value).FirstOrDefault();

            string pageId = await _settingStore.GetSettingValue(Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68"), applicationTenantId);

            var feedResponseItems = await GetFeedResult(client, pageId, facebookPageAccessToken);

            if (feedResponseItems.Count == 0)
            {
                return;
            }

            var eventIds = feedResponseItems.Where(w => !string.IsNullOrEmpty(w.Story) && w.Story.Contains("created an event")).Select(s => s.Id.Replace($"{pageId}_", "")).ToList();

            if (eventIds.Count == 0)
            {
                return;
            }

            var repo = _dataStore.GetRepository<Event>();

            Guid pageTypeId = _dataStore.GetRepository<PageType>().ApplySpecification(new PageTypeFromSystemKeySpecification(PageTypes.EventDetailPageTypeId, applicationTenantId)).Select(s => s.Id).FirstOrDefault();

            foreach (string eventId in eventIds)
            {
                var dbPost = await repo.ApplySpecification(new GetEventByFacebookIdSpecification(eventId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if (dbPost != null)
                {
                    continue;
                }

                string facebookEventJson = await client.GetStringAsync($"{eventId}?access_token={facebookPageAccessToken}&format=json&fields=cover,description,name,place,start_time,end_time");

                var item = JsonSerializer.Deserialize<FacebookEventResult>(facebookEventJson, _jsonSerializerOptions);

                if (item == null)
                {
                    continue;
                }

                string parentSlug = "/events";

                Guid? parentId = null;

                var parentPage = _dataStore.GetRepository<Page>().ApplySpecification(new EventListingPageSpecification()).FirstOrDefault();

                if (parentPage != null)
                {
                    parentSlug = parentPage.Url;
                    parentId = parentPage.Id;
                }

                var newEvent = new Event(applicationTenantId,
                                         pageTypeId: pageTypeId,
                                         parentId: parentId,
                                         parentSlug: parentSlug,
                                         sourceName: "Facebook",
                                         sourceId: item.Id,
                                         title: item.Name ?? "",
                                         description: item.Description,
                                         content: item.Description,
                                         locationName: item.Place?.Name ?? "",
                                         city: item.Place?.Location?.City ?? "",
                                         street: item.Place?.Location?.Street ?? "",
                                         postCode: item.Place?.Location?.Zip ?? "",
                                         country: item.Place?.Location?.Country ?? "",
                                         latitude: Convert.ToDecimal(item.Place?.Location?.Latitude ?? 0d),
                                         longitude: Convert.ToDecimal(item.Place?.Location?.Longitude ?? 0d),
                                         start: item.StartTime,
                                         end: item.EndTime,
                                         imageUrl: item.Cover?.Source ?? "");


                await ConvertImageToLocalImage(newEvent, applicationTenantId, cancellationToken);

                repo.Create(newEvent);

            }

            await _dataStore.SaveChangesAsync();

        }

        private async Task<List<FacebookFeedResponseItem>> GetFeedResult(HttpClient client, string pageId, string accessToken)
        {
            string feedJsonString = await client.GetStringAsync($"{pageId}/feed?access_token={accessToken}&limit=100");

            if (string.IsNullOrEmpty(feedJsonString))
            {
                return new List<FacebookFeedResponseItem>();
            }

            return JsonSerializer.Deserialize<FacebookFeedResponse>(feedJsonString, _jsonSerializerOptions).Data;
        }

        private async Task ConvertImageToLocalImage(Event facebookevent, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string fileName = Path.GetFileName(facebookevent.ImageUrl.Split('?')[0]);

            string fileExt = Path.GetExtension(fileName);

            string friendlyFileName = $"{facebookevent.Title.Replace(" ", "_")}{fileExt}";

            var response = await _clientFactory.CreateClient().GetAsync($"{facebookevent.ImageUrl}");

            response.EnsureSuccessStatusCode();

            await using var stream = await response.Content.ReadAsStreamAsync();

            string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, $"/img/events/{friendlyFileName}", stream, true, cancellationToken);

            facebookevent.SetImageUrl($"/img/events/{friendlyFileName}");

            var bytes = stream.ConvertStreamToByteArray();

            _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCrops(applicationTenantId, finalImagePath, bytes, true));
        }
    }
}
