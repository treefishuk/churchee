using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetPageDetailsRequest : IRequest<GetPageDetailsResponse>
    {
        public GetPageDetailsRequest(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid PageId { get; private set; }

    }


}
