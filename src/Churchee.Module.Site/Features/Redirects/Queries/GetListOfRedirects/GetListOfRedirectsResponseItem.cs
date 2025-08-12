namespace Churchee.Module.Site.Features.Redirects.Queries
{
    public class GetListOfRedirectsResponseItem
    {
        public int Id { get; set; }

        public string Path { get; set; }

        public string RedirectsTo { get; set; }

        public DateTime? Modified { get; set; }
    }
}
