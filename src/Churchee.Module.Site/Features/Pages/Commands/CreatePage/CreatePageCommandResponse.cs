using Churchee.Common.ResponseTypes;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandResponse : CommandResponse
    {
        public CreatePageCommandResponse()
        {

        }

        public CreatePageCommandResponse(Guid pageId)
        {
            PageId = pageId;
        }

        public Guid? PageId { get; set; }
    }
}
