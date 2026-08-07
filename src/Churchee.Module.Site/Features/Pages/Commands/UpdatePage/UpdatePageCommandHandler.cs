using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Hangfire;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandler : IRequestHandler<UpdatePageCommand, CommandResponse>
    {

        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBlobStore _blobStore;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IImageProcessor _imageProcessor;

        public UpdatePageCommandHandler(IDataStore dataStore, ICurrentUser currentUser, IBlobStore blobStore, IBackgroundJobClient backgroundJobClient, IImageProcessor imageProcessor)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
            _blobStore = blobStore;
            _backgroundJobClient = backgroundJobClient;
            _imageProcessor = imageProcessor;
        }


        public async Task<CommandResponse> Handle(UpdatePageCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var page = _dataStore.GetRepository<Page>()
                .ApplySpecification(new PageWithContentAndPropertiesSpecification(request.PageId))
                .First();

            page.UpdateInfo(request.Title, request.Description, request.ParentId, request.Order);
            page.UpdateContent(request.Content);

            string imagePath = await UpdateImageAndReturnPath(request, applicationTenantId, cancellationToken);

            if (!string.IsNullOrEmpty(imagePath))
            {
                page.SetImageUrl(imagePath);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            if (request.Publish)
            {
                page.Publish();
            }

            if (request.Unpublish)
            {
                page.Unpublish();
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task<string> UpdateImageAndReturnPath(UpdatePageCommand request, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string imagePath = string.Empty;

            if (!string.IsNullOrEmpty(request.ImageFileName))
            {
                string fileName = Path.GetFileNameWithoutExtension(request.ImageFileName);

                byte[] data = Convert.FromBase64String(request.Base64Image.Split(',')[1]);

                using var ms = new MemoryStream(data);

                using var webpStream = await _imageProcessor.ConvertToWebP(ms, cancellationToken);

                imagePath = $"/img/pages/{fileName.ToDevName()}.webp";

                string finalImagePath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webpStream, false, cancellationToken);

                _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalImagePath, true, CancellationToken.None));

                return finalImagePath;
            }

            return imagePath;
        }
    }
}
