using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageType
{
    public class CreatePageTypeCommand : IRequest<CommandResponse>
    {
        public CreatePageTypeCommand(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }

    }
}
