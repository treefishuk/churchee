using Churchee.Common.Abstractions.Queries;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{

    public class GetPageTypesListingQuery : GridQueryRequestBase<GetPageTypesListingResponse>
    {
        public GetPageTypesListingQuery(int skip, int take, string searchText, string orderBy, string orderByDirection, int draw)
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
