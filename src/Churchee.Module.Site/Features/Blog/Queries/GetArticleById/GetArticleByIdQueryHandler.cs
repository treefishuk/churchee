using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Queries.GetArticleById
{
    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, GetArticleByIdResponse>
    {

        private readonly IDataStore _dataStore;

        public GetArticleByIdQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<GetArticleByIdResponse> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Article>();

            var article = await repo.FirstOrDefaultAsync(new ArticleFromIdSpecification(request.ArticleId),
                selector: s => new GetArticleByIdResponse
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description,
                    Content = s.Content,
                    CreatedAt = s.CreatedDate,
                    IsPublished = s.Published,
                    PublishOnDate = s.LastPublishedDate,
                    ParentName = s.Parent != null ? s.Parent.Title : string.Empty,
                    ParentId = s.Parent != null ? s.Parent.Id : Guid.Empty,
                },
                cancellationToken: cancellationToken);

            return article;
        }
    }
}
