using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetListingQuery : GridQueryRequestBase<GetListingQueryResponseItem>
    {
        internal GetListingQuery(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {
        }
    }
}
