using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Logging.Features.Queries
{
    internal class GetLogListingQuery : GridQueryRequestBase<GetLogListingResponseItem>
    {
        internal GetLogListingQuery(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {

        }

    }
}
