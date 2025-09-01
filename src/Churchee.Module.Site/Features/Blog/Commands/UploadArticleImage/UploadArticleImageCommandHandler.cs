using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.ImageProcessing.Jobs;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands.UploadArticleImage;
using Churchee.Module.Tenancy.Infrastructure;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Configuration;
using Radzen;

namespace Churchee.Module.Site.Features.Blog.Commands
{
    public class UploadArticleImageCommandHandler : IRequestHandler<UploadArticleImageCommand, UploadArticleImageCommandResponse>
    {
        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IConfiguration _configuration;
        private readonly ITenantResolver _tenantResolver;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IImageProcessor _imageProcessor;

        public UploadArticleImageCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IConfiguration configuration, ITenantResolver tenantResolver, IBackgroundJobClient backgroundJobClient, IImageProcessor imageProcessor)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _configuration = configuration;
            _tenantResolver = tenantResolver;
            _backgroundJobClient = backgroundJobClient;
            _imageProcessor = imageProcessor;
        }

        public async Task<UploadArticleImageCommandResponse> Handle(UploadArticleImageCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string fileName = Path.GetFileNameWithoutExtension(request.FileName).ToDevName();

            string folderPath = await GetFolderPath(applicationTenantId, cancellationToken);

            string filePath = $"/{folderPath.ToDevName()}{fileName.ToDevName()}.webp";

            byte[] bytes = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var stream = new MemoryStream(bytes);

            var imageStream = await _imageProcessor.ResizeImageAsync(stream, request.Width ?? 1920, 0, ".webp", cancellationToken);

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, imageStream, true, default);

            _backgroundJobClient.Enqueue<ImageCropsGenerator>(x => x.CreateCropsAsync(applicationTenantId, finalFilePath, bytes, true, CancellationToken.None));

            return ReturnHtml(request, finalFilePath);
        }

        private UploadArticleImageCommandResponse ReturnHtml(UploadArticleImageCommand request, string finalFilePath)
        {
            string urlPrefix = _configuration.GetSection("Images")["Prefix"];

            string domain = urlPrefix.Replace("*", _tenantResolver.GetTenantDevName());

            string fileNameWithoutExtension = finalFilePath.Replace(".webp", string.Empty);

            var srcSetItems = new List<string>();

            if (request.Width > 1920)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_hd.webp 1920w");
            }

            if (request.Width > 1400)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_xxl.webp 1400w");
            }

            if (request.Width > 1200)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_xl.webp 1200w");
            }

            if (request.Width > 992)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_l.webp 992w");
            }

            if (request.Width > 768)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_m.webp 768w");
            }

            if (request.Width > 576)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_s.webp 576w");
            }

            if (request.Width > 200)
            {
                srcSetItems.Add($"{domain}/{fileNameWithoutExtension}_t.webp 200w");
            }

            if (srcSetItems.Count == 0)
            {
                return new UploadArticleImageCommandResponse
                {
                    ImageHtml = $"<img src=\"{domain}{finalFilePath}\" alt=\"{request.Description}\" class=\"img-fluid\" loading=\"lazy\" width=\"{request.Width ?? 1920}\" crossorigin/>"
                };
            }

            string srcSet = string.Join(", ", srcSetItems);

            return new UploadArticleImageCommandResponse
            {
                ImageHtml = $"<img src=\"{domain}{finalFilePath}\" alt=\"{request.Description}\" class=\"img-fluid\" loading=\"lazy\" srcset=\"{srcSet}\" width=\"{request.Width ?? 1920}\" crossorigin/>"
            };
        }

        private async Task<string> GetFolderPath(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var folderRepo = _dataStore.GetRepository<MediaFolder>();

            var folder = folderRepo.GetQueryable().Where(w => w.Path == "Images/Uploads").FirstOrDefault();

            if (folder != null)
            {
                return folder.Path;
            }

            var imagesFolder = folderRepo.GetQueryable().Where(w => w.Path == "Images/").FirstOrDefault();

            if (imagesFolder == null)
            {
                imagesFolder = new MediaFolder(applicationTenantId, "Images", ".jpg, .png, .webp");

                folderRepo.Create(imagesFolder);

                await _dataStore.SaveChangesAsync(cancellationToken);
            }

            var uploadsFolder = folderRepo.GetQueryable().Where(w => w.Path == "Images/Uploads/").FirstOrDefault();

            if (uploadsFolder == null)
            {
                uploadsFolder = new MediaFolder(applicationTenantId, "Uploads", imagesFolder);

                folderRepo.Create(uploadsFolder);

                await _dataStore.SaveChangesAsync(cancellationToken);
            }

            return uploadsFolder.Path;
        }
    }
}