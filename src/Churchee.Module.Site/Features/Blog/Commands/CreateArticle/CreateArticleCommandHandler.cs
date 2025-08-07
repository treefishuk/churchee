using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class CreateArticleCommandHandler : IRequestHandler<CreateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public CreateArticleCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var pageRepo = _dataStore.GetRepository<Page>();

            var articleRepo = _dataStore.GetRepository<Article>();

            var pageTypeRepo = _dataStore.GetRepository<PageType>();

            var detailPageTypeId = await _dataStore.GetRepository<PageType>().FirstOrDefaultAsync(new PageTypeFromSystemKeySpecification(Helpers.PageTypes.BlogDetailPageTypeId, applicationTenantId), s => s.Id, cancellationToken);

            string parentUrl = await pageRepo.FirstOrDefaultAsync(new PageFromIdSpecification(request.ParentPageId), s => s.Url, cancellationToken);

            string slug = request.Title.ToURL();

            string url = $"{parentUrl}/{slug}";

            var newArticle = new Article(applicationTenantId, detailPageTypeId, request.ParentPageId, request.Title, url, request.Description);

            newArticle.SetContent(request.Content);

            newArticle.SetPublishDate(request.PublishOnDate);

            SuffixGeneration.AddUniqueSuffixIfNeeded(newArticle, _dataStore);

            articleRepo.Create(newArticle);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
