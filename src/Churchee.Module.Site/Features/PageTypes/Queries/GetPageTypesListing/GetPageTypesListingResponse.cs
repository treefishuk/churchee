namespace Churchee.Module.Site.Features.PageTypes.Queries
{
    public class GetPageTypesListingResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DevName { get; set; }

        public bool AllowInRoot { get; set; }
    }
}
