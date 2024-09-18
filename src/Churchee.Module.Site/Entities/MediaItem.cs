using Churchee.Common.Data;

namespace Churchee.Module.Site.Entities
{
    public class MediaItem : Entity
    {
        private MediaItem()
        {

        }

        public MediaItem(Guid applicationTenantId, string title, string url, string description, Guid? mediaFolderId = null) : base(applicationTenantId)
        {

            Title = title;
            Description = description;
            MediaUrl = url;
            MediaFolderId = mediaFolderId;
        }

        public MediaItem(Guid applicationTenantId, string title, string mediaUrl, string description, string html, Guid? mediaFolderId = null, string linkUrl = "", string cssClass = "") : base(applicationTenantId)
        {
            Title = title;
            Description = description;
            MediaUrl = mediaUrl;
            Html = html;
            MediaFolderId = mediaFolderId;
            LinkUrl = linkUrl;
            CssClass = cssClass;
            Order = 10;
        }

        public void UpdateMediaUrl(string mediaUrl)
        {
            if (!string.IsNullOrEmpty(mediaUrl))
            {
                MediaUrl = mediaUrl;
            }
        }

        public void UpdateDetails(string title, string description, string html, string linkUrl, string cssClass, int order)
        {
            if (!string.IsNullOrEmpty(title))
            {
                Title = title;
            }

            if (!string.IsNullOrEmpty(description))
            {
                Description = description;
            }

            if (!string.IsNullOrEmpty(html))
            {
                Html = html;
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                LinkUrl = linkUrl;
            }


            if (!string.IsNullOrEmpty(cssClass))
            {
                CssClass = cssClass;
            }

            Order = order;

        }

        public string Title { get; protected set; }

        public virtual string Description { get; protected set; }

        public virtual string Html { get; protected set; }

        public string MediaUrl { get; protected set; }

        public string LinkUrl { get; protected set; }

        public string CssClass { get; protected set; }

        public Guid? MediaFolderId { get; protected set; }

        public MediaFolder MediaFolder { get; set; }

        public int? Order { get; protected set; }


    }
}
