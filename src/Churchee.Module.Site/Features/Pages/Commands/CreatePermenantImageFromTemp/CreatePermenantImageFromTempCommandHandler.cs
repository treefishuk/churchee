using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.CreatePermenantImageFromTemp
{
    public class CreatePermenantImageFromTempCommandHandler : IRequestHandler<CreatePermenantImageFromTempCommand, string>
    {

        private readonly IImageProcessor _imageProcessor;
        private readonly IBlobStore _blobStore;
        private readonly ICurrentUser _curentUser;
        private readonly IJobService _jobService;

        public CreatePermenantImageFromTempCommandHandler(IImageProcessor imageProcessor, IBlobStore blobStore, ICurrentUser curentUser, IJobService jobService)
        {
            _imageProcessor = imageProcessor;
            _blobStore = blobStore;
            _curentUser = curentUser;
            _jobService = jobService;
        }

        public async Task<string> Handle(CreatePermenantImageFromTempCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _curentUser.GetApplicationTenantId();

            await using var tempFileStream = File.OpenRead(request.UploadRequest.TempFilePath);

            using var webPStream = await _imageProcessor.ConvertToWebP(tempFileStream, cancellationToken);

            string imagePath = $"{request.UploadRequest.FinalFilePath}.webp";

            string webPPath = await _blobStore.SaveAsync(applicationTenantId, imagePath, webPStream, false, cancellationToken);

            File.Delete(request.UploadRequest.TempFilePath);

            _jobService.QueueJob<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, webPPath, true, CancellationToken.None));

            string dir = Path.GetDirectoryName(webPPath);

            string name = Path.GetFileNameWithoutExtension(webPPath);

            string directoryAndName = Path.Combine(dir!, name);

            return await Task.FromResult(directoryAndName);
        }
    }
}
