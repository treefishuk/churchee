using Churchee.Common.ResponseTypes;
using Churchee.Module.Site.Features.PageTypes.Responses;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Queries.GetPageOfPageTypeContent
{
    public class GetContentTypesForPageTypeQuery : IRequest<IEnumerable<GetContentTypesForPageTypeResponse>>
    {
        public GetContentTypesForPageTypeQuery(Guid pageTypeId)
        {
            PageTypeId = pageTypeId;
        }

        public Guid PageTypeId { get; private set; }

       
    }
}
