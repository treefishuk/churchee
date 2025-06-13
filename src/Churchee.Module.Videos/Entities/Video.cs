using Churchee.Module.Site.Entities;

namespace Churchee.Module.Videos.Entities
{
    public class Video : WebContent
    {
        public Video() : base()
        {
            VideoUri = string.Empty;
            SourceName = string.Empty;
            ThumbnailUrl = string.Empty;
        }

        public Video(Guid applicationTenantId, string videoUri, DateTime publishedDate, string sourceName, string sourceId, string title, string description, string thumbnailUrl, string videosPath, Guid pageTypeId)
            : base(applicationTenantId, sourceName, title, title.ToURL(), description)
        {
            VideoUri = videoUri;
            PublishedDate = publishedDate;
            SyncDate = DateTime.Now;
            ThumbnailUrl = thumbnailUrl;
            IsSystem = true;
            PageTypeId = pageTypeId;
            Url = $"/{videosPath.ToLowerInvariant()}/{title.ToURL()}";
            SourceName = sourceName;
            SourceId = sourceId;
            Published = true;
        }

        public string ThumbnailUrl { get; private set; }

        public string VideoUri { get; private set; }

        public DateTime PublishedDate { get; private set; }

        public DateTime SyncDate { get; private set; }

    }
}
