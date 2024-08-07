namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetPageDetailsResponse
    {
        public string Title { get; set; }

        public Guid? ParentId { get; set; }

        public string ParentName { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public bool Published { get; set; }

        public IEnumerable<GetPageDetailsResponseContentItem> ContentItems { get; set; }



    }
}
