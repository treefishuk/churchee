using Churchee.Module.Site.Entities;

namespace Churchee.Module.Podcasts.Entities
{
    public class Podcast : WebContent
    {
        public Podcast() : base()
        {
            AudioUri = string.Empty;
            Source = string.Empty;
            ImageUrl = string.Empty;
            ThumbnailUrl = string.Empty;
            Source = string.Empty;
        }

        public Podcast(Guid applicationTenantId, string audioUri, DateTime publishedDate, string source, string title, string description, string imageUrl, string thumbnailUrl, string podcastsUrl)
            : base(applicationTenantId, source, title, title.ToURL(), description)
        {
            AudioUri = audioUri;
            PublishedDate = publishedDate;
            SyncDate = DateTime.Now;
            ImageUrl = imageUrl;
            ThumbnailUrl = thumbnailUrl;
            IsSystem = true;
            PageTypeId = Guid.Parse("f88412e5-9647-4232-8389-4edf685ecf4e");
            Url = $"/{podcastsUrl.ToLowerInvariant()}/{title.ToURL()}";
        }

        public string AudioUri { get; private set; }

        public string ImageUrl { get; private set; }

        public string ThumbnailUrl { get; private set; }

        public DateTime PublishedDate { get; private set; }

        public DateTime SyncDate { get; private set; }


    }
}
