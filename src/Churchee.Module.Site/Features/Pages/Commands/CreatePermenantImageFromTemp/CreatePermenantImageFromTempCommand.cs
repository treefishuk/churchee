using Churchee.Common.ValueTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePermenantImageFromTemp
{
    public class CreatePermenantImageFromTempCommand : IRequest<string>
    {
        public CreatePermenantImageFromTempCommand(ChunkedUploadRequest request)
        {
            UploadRequest = request;
        }

        public ChunkedUploadRequest UploadRequest { get; set; }
    }
}
