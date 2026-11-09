using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommand : IRequest<CreatePageCommandResponse>
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

        public string ImagePath { get; set; }

        public string ImageFileName { get; set; }

        public string ImageAltTag { get; set; }
    }
}
