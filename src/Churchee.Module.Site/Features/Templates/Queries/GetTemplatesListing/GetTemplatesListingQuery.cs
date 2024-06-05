using Churchee.Common.Abstractions.Queries;
using Churchee.Module.Site.Features.Templates.Responses;

namespace Churchee.Module.Site.Features.Templates.Queries.GetTemplatesListing
{
    public class GetTemplatesListingQuery : GridQueryRequestBase<TemplateListingResponse>
    {
        public GetTemplatesListingQuery(int skip, int take, string searchText, string orderBy, string orderByDirection, int draw)
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
