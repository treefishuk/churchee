using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UpdateArticleCommandHandler : IRequestHandler<UpdateArticleCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly IJobService _jobService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IBlobStore _blobStore;
        private readonly ICurrentUser _currentUser;

        public UpdateArticleCommandHandler(IDataStore dataStore, IJobService jobService, IImageProcessor imageProcessor, IBlobStore blobStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _jobService = jobService;
            _imageProcessor = imageProcessor;
            _blobStore = blobStore;
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

            await using var tempFileStream = File.OpenRead(request.TempImagePath);

            using var webPStream = await _imageProcessor.ConvertToWebP(tempFileStream, cancellationToken);

            string imagePath = Path.Combine(request.ImagePath, $"{Path.GetFileNameWithoutExtension(request.ImageFileName).ToDevName()}.webp");

            string webPPath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webPStream, false, cancellationToken);

            File.Delete(request.TempImagePath);

            _jobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, webPPath, true, CancellationToken.None));

            newArticle.SetImage(webPPath.Replace(".webp", ""), request.ImageAltTag);
        }
    }
}
