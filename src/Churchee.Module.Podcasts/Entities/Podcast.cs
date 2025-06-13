using Churchee.Module.Site.Entities;

namespace Churchee.Module.Podcasts.Entities
{
    public class Podcast : WebContent
    {
        public Podcast() : base()
        {
            AudioUri = string.Empty;
            SourceName = string.Empty;
            ImageUrl = string.Empty;
            ThumbnailUrl = string.Empty;
            Content = string.Empty;
        }

        public Podcast(Guid applicationTenantId, string audioUri, DateTime publishedDate, string sourceName, string sourceId, string title, string description, string imageUrl, string thumbnailUrl, string podcastsUrl, Guid podcastDetailPageTypeId)
            : base(applicationTenantId, sourceName, title, title.ToURL(), description)
        {
            AudioUri = audioUri;
            PublishedDate = publishedDate;
            SyncDate = DateTime.Now;
            ImageUrl = imageUrl;
            ThumbnailUrl = thumbnailUrl;
            IsSystem = true;
            PageTypeId = podcastDetailPageTypeId;
            Url = $"/{podcastsUrl.ToLowerInvariant()}/{title.ToURL()}";
            SourceName = sourceName;
            SourceId = sourceId;
            Content = description;
            Published = true;
        }

        public string? Content { get; private set; }

        public string AudioUri { get; private set; }

        public string ImageUrl { get; private set; }

        public string ThumbnailUrl { get; private set; }

        public DateTime PublishedDate { get; private set; }

        public DateTime SyncDate { get; private set; }

        public void Update(string content, string imageUrl, string thumbnailUrl)
        {
            SyncDate = DateTime.Now;
            Content = content;
            ImageUrl = imageUrl;
            ThumbnailUrl = thumbnailUrl;
        }


    }
}
