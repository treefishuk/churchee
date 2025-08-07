namespace Churchee.Module.Site.Features.Blog.Responses
{
    public class GetListBlogItemsResponseItem
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public bool Published { get; set; }

        public DateTime? Modified { get; set; }
    }
}
