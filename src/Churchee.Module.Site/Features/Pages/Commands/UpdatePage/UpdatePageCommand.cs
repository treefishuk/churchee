using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid? ParentId { get; set; }

        public bool Publish { get; set; }

        public bool Unpublish { get; set; }

        public Guid PageId { get; set; }

        public int Order { get; set; }

        public List<KeyValuePair<Guid, string>> Content { get; set; }

        public List<KeyValuePair<Guid, string>> Properties { get; set; }

        public class Builder
        {
            private readonly UpdatePageCommand _command = new();

            public Builder SetTitle(string title)
            {
                _command.Title = title;
                return this;
            }

            public Builder SetDescription(string description)
            {
                _command.Description = description;
                return this;
            }

            public Builder SetParentId(string parentId)
            {
                _command.ParentId = string.IsNullOrEmpty(parentId) ? null : Guid.Parse(parentId);
                return this;
            }

            public Builder SetPublish(string command)
            {
                if (command == "Unpublish")
                {
                    _command.Unpublish = true;
                }
                if (command == "Publish")
                {
                    _command.Publish = true;
                }

                return this;
            }

            public Builder SetUnpublish(bool unpublish)
            {
                _command.Unpublish = unpublish;
                return this;
            }

            public Builder SetPageId(Guid pageId)
            {
                _command.PageId = pageId;
                return this;
            }

            public Builder SetOrder(int order)
            {
                _command.Order = order;
                return this;
            }

            public Builder SetContent(List<KeyValuePair<Guid, string>> content)
            {
                _command.Content = content;
                return this;
            }

            public Builder SetProperties(List<KeyValuePair<Guid, string>> properties)
            {
                _command.Properties = properties;
                return this;
            }

            public UpdatePageCommand Build()
            {
                return _command;
            }
        }


    }
}
