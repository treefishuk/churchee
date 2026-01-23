using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQuery : GridQueryRequestBase<GetListingQueryResponseItem>
    {
        internal GetListingQuery(int skip, int take, string searchText, string orderBy, Guid? parentId)
            : base(skip, take, searchText, orderBy)
        {
            ParentId = parentId;
        }

        public Guid? ParentId { get; set; }
    }
}
