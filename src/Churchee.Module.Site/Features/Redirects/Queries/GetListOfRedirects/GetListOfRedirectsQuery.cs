using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Site.Features.Redirects.Queries
{
    public class GetListOfRedirectsQuery : GridQueryRequestBase<GetListOfRedirectsResponseItem>
    {
        public GetListOfRedirectsQuery(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {
        }
    }
}
