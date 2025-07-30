namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaFoldersQueryResponseItem
    {
        public GetMediaFoldersQueryResponseItem(Guid id, string title, string path, bool hasChildren, string supportedFileTypes)
        {
            Id = id;
            Title = title;
            Path = path;
            Children = new List<GetMediaFoldersQueryResponseItem>();
            SupportedFileTypes = supportedFileTypes;
            HasChildren = hasChildren;
        }

        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Path { get; set; }

        public bool HasChildren { get; set; }

        public string SupportedFileTypes { get; set; }

        public IEnumerable<GetMediaFoldersQueryResponseItem> Children { get; set; }

    }
}
