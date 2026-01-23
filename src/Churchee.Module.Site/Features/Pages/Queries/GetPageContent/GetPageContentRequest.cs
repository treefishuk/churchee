using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries.GetPageContent
{
    public class GetPageContentRequest : IRequest<IEnumerable<GetPageContentResponseItem>>
    {
        public GetPageContentRequest(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid PageId { get; private set; }

    }


}
