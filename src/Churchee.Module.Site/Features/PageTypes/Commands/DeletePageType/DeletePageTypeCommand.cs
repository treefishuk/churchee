using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.PageTypes.Commands
{
    public class DeletePageTypeCommand : IRequest<CommandResponse>
    {
        public DeletePageTypeCommand(Guid id)
        {
            Id = id;
        }

        public Guid Id { get; set; }

    }
}
