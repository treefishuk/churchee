using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.Validation;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Specifications;
using Hangfire;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommandHandler : IRequestHandler<UpdateMediaItemCommand, CommandResponse>
    {

        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IImageProcessor _imageProcessor;

        public UpdateMediaItemCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IBackgroundJobClient backgroundJobClient, IImageProcessor imageProcessor)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _backgroundJobClient = backgroundJobClient;
            _imageProcessor = imageProcessor;
        }

        public async Task<CommandResponse> Handle(UpdateMediaItemCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            var entity = _dataStore.GetRepository<MediaItem>().ApplySpecification(new SingleMediaItemByIdSpecification(request.Id)).FirstOrDefault();

            if (entity == null)
            {
                response.AddError("Not Found", "Id");

                return response;
            }

            entity.UpdateDetails(request.Name, request.Description, request.AdditionalContent, request.LinkUrl, request.CssClass, request.Order);

            await _dataStore.SaveChangesAsync(cancellationToken);

            if (string.IsNullOrEmpty(request.Base64Content))
            {
                return response;
            }

            string folderPath = _dataStore.GetRepository<MediaFolder>().GetQueryable().Where(w => w.Id == entity.MediaFolderId).Select(s => s.Path).FirstOrDefault() ?? string.Empty;

            byte[] data = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var ms = new MemoryStream(data);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            if (FileValidation.ImageFormats.Contains(request.FileExtension))
            {
                return await HandleImage(request, folderPath, ms, applicationTenantId, entity, cancellationToken);
            }

            return await HandleOtherFormats(request, folderPath, ms, applicationTenantId, entity, cancellationToken);
        }

        private async Task<CommandResponse> HandleImage(UpdateMediaItemCommand request, string folderPath, MemoryStream ms, Guid applicationTenantId, MediaItem entity, CancellationToken cancellationToken)
        {
            string filePath = $"/{folderPath.ToDevName()}{request.FileName.ToDevName()}.webp";

            using var webpStream = await _imageProcessor.ConvertToWebP(ms, cancellationToken);

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, webpStream, true, cancellationToken);

            entity.UpdateMediaUrl(finalFilePath);

            await _dataStore.SaveChangesAsync(cancellationToken);

            _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalFilePath, true, CancellationToken.None));

            return new CommandResponse();
        }

        private async Task<CommandResponse> HandleOtherFormats(UpdateMediaItemCommand request, string folderPath, MemoryStream ms, Guid applicationTenantId, MediaItem entity, CancellationToken cancellationToken)
        {
            string filePath = $"/{folderPath.ToDevName()}{request.FileName.ToDevName()}{request.FileExtension}";

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, ms, true, cancellationToken);

            entity.UpdateMediaUrl(finalFilePath);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
