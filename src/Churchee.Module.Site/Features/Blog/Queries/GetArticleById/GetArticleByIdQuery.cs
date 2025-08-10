using MediatR;

namespace Churchee.Module.Site.Features.Blog.Queries
{
    public class GetArticleByIdQuery : IRequest<GetArticleByIdResponse>
    {
        public GetArticleByIdQuery(Guid articleId)
        {
            ArticleId = articleId;
        }

        public Guid ArticleId { get; set; }
    }
}
