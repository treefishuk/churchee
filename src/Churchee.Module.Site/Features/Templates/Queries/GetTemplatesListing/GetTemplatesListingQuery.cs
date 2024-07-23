using Churchee.Common.Abstractions.Queries;
using Churchee.Module.Site.Features.Templates.Responses;

namespace Churchee.Module.Site.Features.Templates.Queries.GetTemplatesListing
{
    public class GetTemplatesListingQuery : GridQueryRequestBase<TemplateListingResponse>
    {

        internal GetTemplatesListingQuery(int skip, int take, string searchText, string orderBy)
            : base(skip, take, searchText, orderBy)
        {

        }
    }
}
