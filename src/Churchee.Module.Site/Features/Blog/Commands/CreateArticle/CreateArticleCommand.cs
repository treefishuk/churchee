using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class CreateArticleCommand : IRequest<CommandResponse>
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public DateTime? PublishOnDate { get; set; }

        public Guid ParentPageId { get; set; }
    }
}
