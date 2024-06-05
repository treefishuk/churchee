using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQuery : IRequest<IEnumerable<GetListingQueryResponseItem>>
    {
        public GetListingQuery(Guid? parentId, string searchText)
        {
            ParentId = parentId;
            SearchText = searchText;
        }

        public Guid? ParentId { get; set; }

        public string SearchText { get; set; }

    }
}
