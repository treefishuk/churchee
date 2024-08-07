using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommand : IRequest<CommandResponse>
    {
        public UpdatePageCommand(string title, string description, string parentId, bool unpublish, bool publish, Guid pageId, List<KeyValuePair<Guid, string>> content, List<KeyValuePair<Guid, string>> properties)
        {
            Title = title;
            Description = description;
            ParentId = string.IsNullOrEmpty(parentId) ? null : Guid.Parse(parentId);
            Unpublish = unpublish;
            Publish = publish;
            PageId = pageId;
            Content = content;
            Properties = properties;
        }

        public string Title { get; set; }

        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        public bool Publish { get; }

        public bool Unpublish { get; }

        public Guid PageId { get; set; }

        public List<KeyValuePair<Guid, string>> Content { get; }

        public List<KeyValuePair<Guid, string>> Properties { get; }

    }
}
