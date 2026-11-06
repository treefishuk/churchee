using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Jobs;
using Churchee.Module.Site.Specifications;
using Hangfire;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly IJobService _jobService;
        private readonly IImageProcessor _imageProcessor;
        private readonly ICurrentUser _currentUser;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public UpdateArticleCommandHandler(IDataStore dataStore, IJobService jobService, IImageProcessor imageProcessor, ICurrentUser currentUser, IBackgroundJobClient backgroundJobClient)
        {
            _dataStore = dataStore;
            _jobService = jobService;
            _imageProcessor = imageProcessor;
            _currentUser = currentUser;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(UpdateArticleCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var articleRepo = _dataStore.GetRepository<Article>();

            var entity = await articleRepo.FirstOrDefaultAsync(new ArticleFromIdSpecification(request.Id), cancellationToken: cancellationToken);

            entity.UpdateInfo(request.Title, request.Description, request.ImageAltTag, request.ParentPageId);
            entity.SetContent(request.Content);
            entity.SetPublishDate(request.PublishOnDate);

            await ProcessImages(request, applicationTenantId, entity, cancellationToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }


        private async Task ProcessImages(UpdateArticleCommand request, Guid applicationTenantId, Article page, CancellationToken cancellationToken)
        {
            if (request.ImageBytes == null || string.IsNullOrEmpty(request.ImageFileName))
            {
                return;
            }

            _backgroundJobClient.Enqueue<ArticleImageProcessingJob>(x => x.ProcessAsync(applicationTenantId, page.Id, request.ImageFileName, request.ImageBytes, CancellationToken.None));
        }
    }
}
