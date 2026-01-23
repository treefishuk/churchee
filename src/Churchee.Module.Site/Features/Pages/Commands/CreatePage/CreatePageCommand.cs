using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommand : IRequest<CommandResponse>
    {
        public CreatePageCommand(string title, string description, string pageTypeId, string parentId)
        {
            Title = title;
            Description = description;
            PageTypeId = Guid.Parse(pageTypeId);
            ParentId = string.IsNullOrEmpty(parentId) ? null : Guid.Parse(parentId);
        }

        public string Title { get; }

        public string Description { get; }

        public Guid? ParentId { get; }

        public Guid PageTypeId { get; }

    }
}
