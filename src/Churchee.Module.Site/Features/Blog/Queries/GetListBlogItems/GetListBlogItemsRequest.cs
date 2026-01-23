using Churchee.Common.Abstractions.Queries;
using Churchee.Module.Site.Features.Blog.Responses;

namespace Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems
{

    public class GetListBlogItemsRequest : GridQueryRequestBase<GetListBlogItemsResponseItem>
    {
        internal GetListBlogItemsRequest(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {
        }
    }
}
