using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Helpers;
using Churchee.Module.Podcasts.Specifications;
using Churchee.Module.Podcasts.Spotify.Exceptions;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnablePodcasts;
using Churchee.Module.Podcasts.Spotify.Specifications;
using Churchee.Module.Site.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Commands.EnableSpotifyPodcastSync
{
    public class EnableSpotifyPodcastsSyncCommandHandler : IRequestHandler<EnableSpotifyPodcastSyncCommand, CommandResponse>
    {
        private readonly Guid _rssFeedSettingKey = Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a");
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _settingStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IImageProcessor _imageProcessor;
        private readonly IJobService _jobService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger _logger;

        public EnableSpotifyPodcastsSyncCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IDataStore dataStore, IBlobStore blobStore, IImageProcessor imageProcessor, IJobService jobService, IHttpClientFactory httpClientFactory, ILogger<EnableSpotifyPodcastsSyncCommandHandler> logger)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _dataStore = dataStore;
            _blobStore = blobStore;
            _imageProcessor = imageProcessor;
            _jobService = jobService;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(EnableSpotifyPodcastSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(_rssFeedSettingKey, applicationTenantId, $"SpotifyRSSFeedUrl", request.SpotifyFMRSSFeed);

            string podcastsUrl = await _settingStore.GetSettingValue(_podcastsNameId, applicationTenantId);

            _jobService.ScheduleJob($"{applicationTenantId}_SpotifyPodcasts", () => SyncPodcasts(request, applicationTenantId, podcastsUrl, CancellationToken.None), Cron.Hourly);

            _jobService.QueueJob(() => SyncPodcasts(request, applicationTenantId, podcastsUrl, CancellationToken.None));

            return new CommandResponse();
        }

        [DisableConcurrentExecution(timeoutInSeconds: 600)]
        public async Task SyncPodcasts(EnableSpotifyPodcastSyncCommand request, Guid applicationTenantId, string podcastsUrl, CancellationToken cancellationToken)
        {
            var podcastShows = await GetAndParseRssFeed(request);

            await AddOrUpdatePodcasts(applicationTenantId, podcastShows, podcastsUrl, cancellationToken);

            await _dataStore.SaveChangesAsync(cancellationToken);
        }

        private async Task AddOrUpdatePodcasts(Guid applicationTenantId, RssChannelItem[] podcastShows, string podcastsUrl, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Podcast>();

            var podcasts = new List<Podcast>();

            foreach (var item in podcastShows)
            {
                string audioUri = item.Enclosure.Url;

                bool alreadyExists = repo.AnyWithFiltersDisabled(w => w.AudioUri == audioUri && w.ApplicationTenantId == applicationTenantId);

                if (alreadyExists)
                {
                    await UpdateExistingPodcast(applicationTenantId, audioUri, repo, item, cancellationToken);
                }

                if (!alreadyExists)
                {

                    await AddNewPodcast(applicationTenantId, podcastsUrl, podcasts, item, cancellationToken);
                }
            }

            if (podcasts.Count > 0)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    foreach (var podcast in podcasts)
                    {
                        _logger.LogInformation("Adding new podcast with audio URL: {AudioUri}", podcast.AudioUri);
                    }
                }

                repo.AddRange(podcasts);
            }
        }

        private async Task UpdateExistingPodcast(Guid applicationTenantId, string audioUri, IRepository<Podcast> repository, RssChannelItem item, CancellationToken cancellationToken)
        {
            var existing = await repository.FirstOrDefaultAsync(new PodcastByAudioUrlSpecification(audioUri, applicationTenantId), cancellationToken);

            if (existing == null)
            {
                return;
            }

            string fileName = Path.GetFileName(item.Image.Href);

            string thumbFileName = $"{Path.GetFileNameWithoutExtension(item.Image.Href)}_t{Path.GetExtension(item.Image.Href)}";

            if (existing.ImageUrl != $"/img/audio/{fileName}")
            {
                await GenerateImage(applicationTenantId, item.Image.Href, fileName, thumbFileName, cancellationToken);
            }

            existing.Update(item.Description, $"/img/audio/{fileName}", $"/img/audio/{thumbFileName}");
        }

        private async Task GenerateImage(Guid applicationTenantId, string sourceImageUrl, string fileName, string thumbFileName, CancellationToken cancellationToken)
        {

            string ext = Path.GetExtension(fileName);

            using var httpClient = _httpClientFactory.CreateClient();

            var imageStream = await httpClient.GetStreamAsync(sourceImageUrl, cancellationToken);

            var resizedImageStream = await _imageProcessor.ResizeImageAsync(imageStream, 350, 0, ext, cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{fileName}", resizedImageStream, true, cancellationToken);

            var originalImgStream = await _blobStore.GetReadStreamAsync(applicationTenantId, $"/img/audio/{fileName}", cancellationToken);

            var thumbnailImage = await _imageProcessor.ResizeImageAsync(originalImgStream, 50, 0, ext, cancellationToken);

            await _blobStore.SaveAsync(applicationTenantId, $"/img/audio/{thumbFileName}", thumbnailImage, true, cancellationToken);
        }


        private async Task AddNewPodcast(Guid applicationTenantId, string podcastsUrl, List<Podcast> podcasts, RssChannelItem item, CancellationToken cancellationToken)
        {
            var podcastDetailPageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(PageTypes.PodcastDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            if (podcastDetailPageTypeId == Guid.Empty)
            {
                throw new PodcastSyncException("podcastDetailPageTypeId is Empty");
            }

            string fileName = Path.GetFileName(item.Image.Href);

            string thumbFileName = $"{Path.GetFileNameWithoutExtension(item.Image.Href)}_t{Path.GetExtension(item.Image.Href)}";

            await GenerateImage(applicationTenantId, item.Image.Href, fileName, thumbFileName, cancellationToken);

            podcasts.Add(new Podcast(applicationTenantId: applicationTenantId,
                audioUri: item.Enclosure.Url,
                publishedDate: DateTime.Parse(item.PubDate, new CultureInfo("en-GB")),
                sourceName: "Spotify",
                sourceId: item.Guid.Value,
                title: item.Title,
                description: item.Description,
                imageUrl: $"/img/audio/{fileName}",
                thumbnailUrl: $"/img/audio/{thumbFileName}",
                podcastsUrl: podcastsUrl,
                podcastDetailPageTypeId: podcastDetailPageTypeId));
        }

        private async Task<RssChannelItem[]> GetAndParseRssFeed(EnableSpotifyPodcastSyncCommand request)
        {
            var client = _httpClientFactory.CreateClient();

            string xml = await client.GetStringAsync(request.SpotifyFMRSSFeed);

            var doc = XDocument.Parse(xml);

            if (doc == null || doc.Root == null)
            {
                throw new InvalidOperationException("The RSS feed document is null or has no root element.");
            }

            var serializer = new XmlSerializer(typeof(Rss));

            var reader = doc.Root.CreateReader();

            var feed = (Rss?)serializer.Deserialize(reader);

            if (feed == null)
            {
                throw new InvalidOperationException("Failed to deserialize the RSS feed.");
            }

            var items = feed.Channel.Items;

            return items;
        }
    }
}
