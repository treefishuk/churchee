using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UnPublishArticleCommand : IRequest<CommandResponse>
    {
        public UnPublishArticleCommand(Guid articleId)
        {
            ArticleId = articleId;
        }

        public Guid ArticleId { get; }
    }
}
