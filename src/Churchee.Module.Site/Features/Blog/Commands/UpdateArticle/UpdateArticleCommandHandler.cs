using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly IJobService _jobService;
        private readonly IImageProcessor _imageProcessor;
        private readonly ICurrentUser _currentUser;

        public UpdateArticleCommandHandler(IDataStore dataStore, IJobService jobService, IImageProcessor imageProcessor, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _jobService = jobService;
            _imageProcessor = imageProcessor;
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

            await ProcessImage(request, applicationTenantId, entity, cancellationToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }


        private async Task ProcessImage(UpdateArticleCommand request, Guid applicationTenantId, Article newArticle, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.TempImagePath) || string.IsNullOrEmpty(request.ImageFileName))
            {
                return;
            }

            string imagePath = await _imageProcessor.ConvertTempImageToFullImage(request.TempImagePath, request.ImageFileName, request.ImagePath, applicationTenantId, cancellationToken);

            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            _jobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, imagePath, true, CancellationToken.None));

            newArticle.SetImageInfo(imagePath.Replace(".webp", ""), request.ImageAltTag);
        }
    }
}
