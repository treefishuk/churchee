using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

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
                    ImageAltTag = s.ImageAltTag,
                    ImageThumbnail = string.IsNullOrEmpty(s.ImageUrl) ? "/_content/Churchee.Module.UI/img/opengraph-placeholder.png" : s.ImageUrl + "_t.webp",
                },
                cancellationToken: cancellationToken);

            return article;
        }
    }
}
