using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.Validation;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Hangfire;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaItemCommandHandler : IRequestHandler<CreateMediaItemCommand, CommandResponse>
    {
        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IImageProcessor _imageProcessor;

        public CreateMediaItemCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IBackgroundJobClient backgroundJobClient, IImageProcessor imageProcessor)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _backgroundJobClient = backgroundJobClient;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(CreateMediaItemCommand request, CancellationToken cancellationToken)
        {
            string folderPath = _dataStore.GetRepository<MediaFolder>().GetQueryable().Where(w => w.Id == request.FolderId).Select(s => s.Path).FirstOrDefault() ?? string.Empty;

            byte[] data = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var ms = new MemoryStream(data);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            if (FileValidation.ImageFormats.Contains(request.FileExtension))
            {
                return await HandleImage(request, folderPath, ms, applicationTenantId, cancellationToken);
            }

            else
            {
                return await HandleOtherFormats(request, folderPath, ms, applicationTenantId, cancellationToken);
            }
        }

        private async Task<CommandResponse> HandleOtherFormats(CreateMediaItemCommand request, string folderPath, MemoryStream ms, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string filePath = $"/{folderPath.ToDevName()}{request.FileName.ToDevName()}{request.FileExtension}";

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, ms, true, cancellationToken);

            var media = new MediaItem(applicationTenantId, request.Name, finalFilePath, request.Description, request.AdditionalContent, request.FolderId, request.LinkUrl, request.CssClass);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task<CommandResponse> HandleImage(CreateMediaItemCommand request, string folderPath, MemoryStream ms, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            string filePath = $"/{folderPath.ToDevName()}{request.FileName.ToDevName()}.webp";

            using var webpStream = await _imageProcessor.ConvertToWebP(ms, cancellationToken);

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, webpStream, true, cancellationToken);

            var media = new MediaItem(applicationTenantId, request.Name, finalFilePath, request.Description, request.AdditionalContent, request.FolderId, request.LinkUrl, request.CssClass);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalFilePath, true, CancellationToken.None));

            return new CommandResponse();

        }
    }
}
