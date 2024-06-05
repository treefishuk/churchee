using Churchee.Common.Abstractions.Queries;
using Churchee.Module.Site.Features.Blog.Responses;

namespace Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems
{

    public class GetListBlogItemsRequest : GridQueryRequestBase<GetListBlogItemsResponseItem>
    {
        public GetListBlogItemsRequest(int skip, int take, string searchText, string orderBy, string orderByDirection, int draw)
        {
            Skip = skip;
            Take = take;
            SearchText = searchText;
            OrderBy = orderBy;
            OrderByDirection = orderByDirection;
            Draw = draw;
        }
    }
}
