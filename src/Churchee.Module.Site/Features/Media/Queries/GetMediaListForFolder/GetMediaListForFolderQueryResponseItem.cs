namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaListForFolderQueryResponseItem
    {
        public GetMediaListForFolderQueryResponseItem(Guid id, string title, string description, string additionalContent, string mediaUrl, string linkUrl)
        {
            Id = id;
            Title = title;
            MediaUrl = mediaUrl;
            Description = description;
            AdditionalContent = additionalContent;
            LinkUrl = linkUrl;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AdditionalContent { get; set; }

        public string MediaUrl { get; set; }

        public string LinkUrl { get; set; }
    }
}
