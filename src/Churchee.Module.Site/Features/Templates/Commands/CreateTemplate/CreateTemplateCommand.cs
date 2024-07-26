using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Templates.Commands
{
    public class CreateTemplateCommand : IRequest<CommandResponse>
    {
        public CreateTemplateCommand(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public string Path { get; set; }

        public string Content { get; set; }
    }
}
