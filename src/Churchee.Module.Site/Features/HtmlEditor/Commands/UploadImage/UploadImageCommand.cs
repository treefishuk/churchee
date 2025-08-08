using MediatR;
using Microsoft.AspNetCore.Http;

namespace Churchee.Module.Site.Features.HtmlEditor.Commands
{
    public class UploadImageCommand : IRequest<string>
    {
        public UploadImageCommand(IFormFile file)
        {
            File = file;
        }

        public IFormFile File { get; set; }
    }
}
