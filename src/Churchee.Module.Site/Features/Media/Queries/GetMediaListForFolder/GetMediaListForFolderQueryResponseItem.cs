namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaListForFolderQueryResponseItem
    {
        public GetMediaListForFolderQueryResponseItem(Guid id, string title, string description, string additionalContent, string mediaUrl, string linkUrl, string cssClass)
        {
            Id = id;
            Title = title;
            MediaUrl = mediaUrl;
            Description = description;
            AdditionalContent = additionalContent;
            LinkUrl = linkUrl;
            CssClass = cssClass;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string MediaUrl { get; set; }

        public string MediaUrlSmall
        {
            get
            {
                var fileName = Path.GetFileNameWithoutExtension(MediaUrl);

                return MediaUrl.Replace(fileName, $"{fileName}_s");
            }
        }

        public string LinkUrl { get; set; }

        public string CssClass { get; set; }

    }
}
