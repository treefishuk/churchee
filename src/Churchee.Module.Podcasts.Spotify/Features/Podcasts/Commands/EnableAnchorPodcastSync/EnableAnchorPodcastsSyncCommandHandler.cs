using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Helpers;
using Churchee.Module.Podcasts.Spotify.Exceptions;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnablePodcasts;
using Churchee.Module.Podcasts.Spotify.Specifications;
using Churchee.Module.Site.Entities;
using Hangfire;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands
{
    public class EnableSpotifyPodcastsSyncCommandHandler : IRequestHandler<EnableSpotifyPodcastSyncCommand, CommandResponse>
    {
        private readonly Guid _rssFeedSettingKey = Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a");
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;

        public EnableSpotifyPodcastsSyncCommandHandler(ISettingStore settingStore, IRecurringJobManager recurringJobManager, ICurrentUser currentUser, IDataStore dataStore, IBackgroundJobClient backgroundJobClient, IBlobStore blobStore, IImageProcessor imageProcessor)
        {
            _settingStore = settingStore;
            _recurringJobManager = recurringJobManager;
            _currentUser = currentUser;
            _dataStore = dataStore;
            _backgroundJobClient = backgroundJobClient;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(EnableSpotifyPodcastSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(_rssFeedSettingKey, applicationTenantId, $"SpotifyRSSFeedUrl", request.SpotifyFMRSSFeed);

            string podcastsUrl = await _settingStore.GetSettingValue(_podcastsNameId, applicationTenantId);

            _recurringJobManager.AddOrUpdate($"{applicationTenantId}_SpotifyPodcasts", () => SyncPodcasts(request, applicationTenantId, podcastsUrl), Cron.Daily);

            _backgroundJobClient.Enqueue(() => SyncPodcasts(request, applicationTenantId, podcastsUrl));

            return new CommandResponse();
        }

        public async Task SyncPodcasts(EnableSpotifyPodcastSyncCommand request, Guid applicationTenantId, string podcastsUrl)
        {
            var podcastShows = await GetAndParseRssFeed(request);

            await AddOrUpdatePodcasts(applicationTenantId, podcastShows, podcastsUrl);

            await _dataStore.SaveChangesAsync();
        }

        private async Task AddOrUpdatePodcasts(Guid applicationTenantId, rssChannelItem[] podcastShows, string podcastsUrl)
        {
            var repo = _dataStore.GetRepository<Podcast>();

            var podcasts = new List<Podcast>();

            foreach (var item in podcastShows)
            {
                var audioUri = item.enclosure.url;

                var alreadyExists = repo.AnyWithFiltersDisabled(w => w.AudioUri == audioUri && w.ApplicationTenantId == applicationTenantId);

                if (alreadyExists)
                {
                    await UpdateExistingPodcast(applicationTenantId, audioUri, repo, item);
                }

                if (!alreadyExists)
                {
                    await AddNewPodcast(applicationTenantId, podcastsUrl, podcasts, item);
                }
            }

            if (podcasts.Count > 0)
            {
                repo.AddRange(podcasts);
            }
        }

        private async Task UpdateExistingPodcast(Guid applicationTenantId, string audioUri, IRepository<Podcast> repository, rssChannelItem item)
        {
            var existing = await repository.GetQueryable().Where(w => w.AudioUri == audioUri).FirstOrDefaultAsync();

            if (existing == null)
            {
                return;
            }

            string fileName = Path.GetFileName(item.image.href);

            string thumbFileName = $"{Path.GetFileNameWithoutExtension(item.image.href)}_t{Path.GetExtension(item.image.href)}";

            if (existing.ImageUrl != $"/img/audio/{fileName}")
            {
                await GenerateImage(applicationTenantId, item.image.href, fileName, thumbFileName);
            }

            existing.Update(item.description, $"/img/audio/{fileName}", $"/img/audio/{thumbFileName}");
        }

        private async Task GenerateImage(Guid applicationTenantId, string sourceImageUrl, string fileName, string thumbFileName)
        {

            string ext = Path.GetExtension(fileName);

            using var httpClient = new HttpClient();

            var imageStream = await httpClient.GetStreamAsync(sourceImageUrl);

            var resizedImageStream = _imageProcessor.ResizeImage(imageStream, 350, 0, ext);

            await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{fileName}", resizedImageStream, true, default);

            var originalImgStream = await _blobStore.GetAsync(applicationTenantId, $"/img/audio/{fileName}");

            var thumbnailImage = _imageProcessor.ResizeImage(originalImgStream, 50, 0, ext);

            await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{thumbFileName}", thumbnailImage, true, default);
        }


        private async Task AddNewPodcast(Guid applicationTenantId, string podcastsUrl, List<Podcast> podcasts, rssChannelItem item)
        {
            Guid podcastDetailPageTypeId = await _dataStore.GetRepository<PageType>().ApplySpecification(new PageTypeFromSystemKeySpecification(PageTypes.PodcastDetailPageTypeId, applicationTenantId)).Select(s => s.Id).FirstOrDefaultAsync();

            if (podcastDetailPageTypeId == Guid.Empty)
            {
                throw new PodcastSyncException("podcastDetailPageTypeId is Empty");
            }

            string fileName = Path.GetFileName(item.image.href);

            string thumbFileName = $"{Path.GetFileNameWithoutExtension(item.image.href)}_t{Path.GetExtension(item.image.href)}";

            await GenerateImage(applicationTenantId, item.image.href, fileName, thumbFileName);

            podcasts.Add(new Podcast(applicationTenantId: applicationTenantId,
                audioUri: item.enclosure.url,
                publishedDate: DateTime.Parse(item.pubDate),
                sourceName: "Spotify",
                sourceId: item.guid.Value,
                title: item.title,
                description: item.description,
                imageUrl: $"/img/audio/{fileName}",
                thumbnailUrl: $"/img/audio/{thumbFileName}",
                podcastsUrl: podcastsUrl,
                podcastDetailPageTypeId: podcastDetailPageTypeId));
        }

        private static async Task<rssChannelItem[]> GetAndParseRssFeed(EnableSpotifyPodcastSyncCommand request)
        {
            var client = new HttpClient();

            string xml = await client.GetStringAsync(request.SpotifyFMRSSFeed);

            var doc = XDocument.Parse(xml);

            if (doc == null || doc.Root == null)
            {
                throw new NullReferenceException(nameof(doc));
            }

            var serializer = new XmlSerializer(typeof(rss));

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
