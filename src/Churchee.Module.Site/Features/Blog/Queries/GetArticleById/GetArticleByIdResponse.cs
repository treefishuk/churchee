namespace Churchee.Module.Site.Features.Blog.Queries
{
    public class GetArticleByIdResponse
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }

        public bool IsPublished { get; set; }

        public DateTime? PublishOnDate { get; set; }
    }
}
