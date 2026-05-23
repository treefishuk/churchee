using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class DeleteArticleCommand : IRequest<CommandResponse>
    {
        public DeleteArticleCommand(Guid articleId)
        {
            ArticleId = articleId;
        }

        public Guid ArticleId { get; private set; }

    }
}
