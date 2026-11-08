using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public CreateArticleCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser; ;
        }

        public async Task<CommandResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var articleRepo = _dataStore.GetRepository<Article>();

            var detailPageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(Helpers.PageTypes.BlogDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            string url = await GetSlug(request, cancellationToken);

            var newArticle = new Article(applicationTenantId, detailPageTypeId, request.ParentPageId, request.Title, url, request.Description);

            newArticle.SetContent(request.Content);

            newArticle.SetPublishDate(request.PublishOnDate);

            newArticle.SetImageInfo(request.ImageFileName, request.ImageAltTag);

            SuffixGeneration.AddUniqueSuffixIfNeeded(newArticle, _dataStore.GetRepository<Article>());

            articleRepo.Create(newArticle);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task<string> GetSlug(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var pageRepo = _dataStore.GetRepository<Page>();

            string parentUrl = await pageRepo.FirstOrDefaultAsync(new PageFromIdSpecification(request.ParentPageId), s => s.Url, cancellationToken);

            string slug = request.Title.ToURL();

            return $"{parentUrl}/{slug}";
        }
    }
}
