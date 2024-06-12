using System.Xml.Linq;
using System.Xml.Serialization;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Anchor.Features.Podcasts.Commands.EnablePodcasts;
using Churchee.Module.Podcasts.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Podcasts.Anchor.Features.Podcasts.Commands
{
    public class EnableAnchorPodcastsSyncCommandHandler : IRequestHandler<EnableAnchorPodcastSyncCommand, CommandResponse>
    {
        private readonly Guid _rssFeedSettingKey = Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a");
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<EnableAnchorPodcastsSyncCommandHandler> _logger;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;

        public EnableAnchorPodcastsSyncCommandHandler(ISettingStore settingStore, IRecurringJobManager recurringJobManager, ICurrentUser currentUser, ILogger<EnableAnchorPodcastsSyncCommandHandler> logger, IDataStore dataStore, IBackgroundJobClient backgroundJobClient, IBlobStore blobStore, IImageProcessor imageProcessor)
        {
            _settingStore = settingStore;
            _recurringJobManager = recurringJobManager;
            _currentUser = currentUser;
            _logger = logger;
            _dataStore = dataStore;
            _backgroundJobClient = backgroundJobClient;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(EnableAnchorPodcastSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(_rssFeedSettingKey, applicationTenantId, $"AnchorRSSFeedUrl", request.AnchorFMRSSFeed);

            string podcastsUrl = await _settingStore.GetSettingValue(_podcastsNameId, applicationTenantId);

            _recurringJobManager.AddOrUpdate($"{applicationTenantId}_AnchorPodcasts", () => SyncPodcasts(request, applicationTenantId, podcastsUrl), Cron.Daily);

            _backgroundJobClient.Enqueue(() => SyncPodcasts(request, applicationTenantId, podcastsUrl));

            return new CommandResponse();
        }

        public async Task SyncPodcasts(EnableAnchorPodcastSyncCommand request, Guid applicationTenantId, string podcastsUrl)
        {
            var podcastShows = await GetAndParseRssFeed(request);

            await AddPodcastShowsNotYetAdded(applicationTenantId, podcastShows, podcastsUrl);

            await _dataStore.SaveChangesAsync();
        }

        private async Task AddPodcastShowsNotYetAdded(Guid applicationTenantId, rssChannelItem[] podcastShows, string podcastsUrl)
        {
            var repo = _dataStore.GetRepository<Podcast>();

            var podcasts = new List<Podcast>();

            foreach (var item in podcastShows)
            {
                var audioUri = item.enclosure.url;

                var alreadyExists = repo.AnyWithFiltersDisabled(w => w.AudioUri == audioUri && w.ApplicationTenantId == applicationTenantId);

                if (!alreadyExists)
                {
                    using var httpClient = new HttpClient();

                    var imageStream = await httpClient.GetStreamAsync(item.image.href);

                    var resizedImageStream = _imageProcessor.ResizeImage(imageStream, 350, 0);

                    string fileName = Path.GetFileName(item.image.href);

                    await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{fileName}", resizedImageStream, true, default);

                    string thumbFileName = $"{Path.GetFileNameWithoutExtension(item.image.href)}_t{Path.GetExtension(item.image.href)}";

                    var originalImgStream = await _blobStore.GetAsync(applicationTenantId, $"/img/audio/{fileName}");

                    var thumbnailImage = _imageProcessor.ResizeImage(originalImgStream, 50, 0);

                    await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{thumbFileName}", thumbnailImage, true, default);

                    podcasts.Add(new Podcast(applicationTenantId: applicationTenantId, 
                        audioUri: item.enclosure.url, 
                        publishedDate: DateTime.Parse(item.pubDate), 
                        sourceName: "Spotify",
                        sourceId: item.guid.Value,
                        title: item.title,
                        description: item.description,
                        imageUrl: $"/img/audio/{fileName}",
                        thumbnailUrl: $"/img/audio/{thumbFileName}",
                        podcastsUrl: podcastsUrl));
                }
            }

            if (podcasts.Count > 0)
            {
                repo.AddRange(podcasts);
            }
        }

        private static async Task<rssChannelItem[]> GetAndParseRssFeed(EnableAnchorPodcastSyncCommand request)
        {
            var client = new HttpClient();

            string xml = await client.GetStringAsync(request.AnchorFMRSSFeed);

            var doc = XDocument.Parse(xml);

            if (doc == null || doc.Root == null)
            {
                throw new NullReferenceException(nameof(doc));
            }

            var serializer = new XmlSerializer(typeof(rss));

            if (serializer == null)
            {
                throw new NullReferenceException(nameof(serializer));
            }

            var reader = doc.Root.CreateReader();

            object? temp = serializer.Deserialize(reader);

            if (temp == null)
            {
                throw new NullReferenceException(nameof(temp));
            }

            var items = ((rss)temp).channel.Items;

            return items;
        }
    }
}
