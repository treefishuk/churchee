using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class PublishArticleCommand : IRequest<CommandResponse>
    {
        public PublishArticleCommand(Guid articleId)
        {
            ArticleId = articleId;
        }

        public Guid ArticleId { get; }
    }
}
