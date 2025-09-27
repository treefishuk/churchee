using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
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
        private readonly IJobService _jobService;
        private readonly IImageProcessor _imageProcessor;
        private readonly IBlobStore _blobStore;

        public CreateArticleCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IJobService jobService, IImageProcessor imageProcessor, IBlobStore blobStore)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _jobService = jobService;
            _imageProcessor = imageProcessor;
            _blobStore = blobStore;
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

            await ProcessImage(request, applicationTenantId, newArticle, cancellationToken);

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

        private async Task ProcessImage(CreateArticleCommand request, Guid applicationTenantId, Article newArticle, CancellationToken cancellationToken)
        {
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
