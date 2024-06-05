using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommand : IRequest<CommandResponse>
    {
        public CreatePageCommand(string title, string desciption, string pageTypeId, string parentId)
        {
            Title = title;
            Description = desciption;
            PageTypeId = Guid.Parse(pageTypeId);
            ParentId = parentId != null ? Guid.Parse(parentId) : null;
        }

        public string Title { get; }

        public string Description { get; }

        public Guid? ParentId { get; }

        public Guid PageTypeId { get; }

    }
}
