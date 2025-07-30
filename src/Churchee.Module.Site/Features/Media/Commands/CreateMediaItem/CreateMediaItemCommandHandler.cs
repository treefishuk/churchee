using Churchee.Common.Abstractions.Auth;
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

        public CreateMediaItemCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IBackgroundJobClient backgroundJobClient)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<CommandResponse> Handle(CreateMediaItemCommand request, CancellationToken cancellationToken)
        {
            string folderPath = _dataStore.GetRepository<MediaFolder>().GetQueryable().Where(w => w.Id == request.FolderId).Select(s => s.Path).FirstOrDefault() ?? string.Empty;

            byte[] data = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var ms = new MemoryStream(data);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string filePath = $"/{folderPath.ToDevName()}{request.FileName.ToDevName()}{request.FileExtension}";

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, ms, true, cancellationToken);

            var media = new MediaItem(applicationTenantId, request.Name, filePath, request.Description, request.AdditionalContent, request.FolderId, request.LinkUrl, request.CssClass);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            if (!FileValidation.IsImageFile(finalFilePath))
            {
                return new CommandResponse();
            }

            byte[] bytes = ms.ConvertStreamToByteArray();

            _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCrops(applicationTenantId, finalFilePath, bytes, true));

            return new CommandResponse();
        }

    }
}
