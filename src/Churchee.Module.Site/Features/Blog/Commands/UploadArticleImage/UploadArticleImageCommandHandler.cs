using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Commands.UploadArticleImage;
using Churchee.Module.Tenancy.Infrastructure;
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

        public UploadArticleImageCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IConfiguration configuration, ITenantResolver tenantResolver)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _configuration = configuration;
            _tenantResolver = tenantResolver;
        }

        public async Task<UploadArticleImageCommandResponse> Handle(UploadArticleImageCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string fileName = Path.GetFileNameWithoutExtension(request.FileName).ToDevName();

            var (folderPath, folderId) = await GetFolderPath(applicationTenantId, cancellationToken);

            string filePath = $"/{folderPath.ToDevName()}{fileName.ToDevName()}{request.FileExtension}";

            byte[] data = Convert.FromBase64String(request.Base64Content.Split(',')[1]);

            using var ms = new MemoryStream(data);

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, ms, true, cancellationToken);

            var media = new MediaItem(applicationTenantId, fileName.ToSentence(), filePath, string.Empty, string.Empty, folderId, string.Empty, string.Empty);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            string urlPrefix = _configuration.GetSection("Images")["Prefix"];

            string domain = urlPrefix.Replace("*", _tenantResolver.GetTenantDevName());

            return new UploadArticleImageCommandResponse
            {
                ImageHtml = $"<img src=\"{domain}{finalFilePath}\" alt=\"{request.Description}\" class=\"img-fluid\" />"
            };
        }

        private async Task<(string folderPath, Guid folderId)> GetFolderPath(Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var folderRepo = _dataStore.GetRepository<MediaFolder>();

            var folder = folderRepo.GetQueryable().Where(w => w.Path == "Images/Uploads").FirstOrDefault();

            if (folder != null)
            {
                return (folder.Path, folder.Id);
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

            return (uploadsFolder.Path, uploadsFolder.Id);
        }
    }
}