using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public UpdateArticleCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var articleRepo = _dataStore.GetRepository<Article>();

            var entity = await articleRepo.FirstOrDefaultAsync(new ArticleFromIdSpecification(request.Id), cancellationToken: cancellationToken);

            entity.UpdateInfo(request.Title, request.Description, request.ImageAltTag, request.ParentPageId);
            entity.SetContent(request.Content);
            entity.SetPublishDate(request.PublishOnDate);

            entity.SetImageInfo(string.IsNullOrEmpty(request.ImagePath) ? entity.ImageUrl : request.ImagePath, request.ImageAltTag);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }



    }
}
