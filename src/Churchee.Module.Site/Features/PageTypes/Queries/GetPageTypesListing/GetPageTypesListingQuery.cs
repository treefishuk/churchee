using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{

    public class GetPageTypesListingQuery : GridQueryRequestBase<GetPageTypesListingResponse>
    {
        internal GetPageTypesListingQuery(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {
        }
    }
}
