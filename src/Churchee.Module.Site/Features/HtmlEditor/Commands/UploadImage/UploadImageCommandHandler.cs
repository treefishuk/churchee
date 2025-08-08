using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Infrastructure;
using MediatR;
using Microsoft.Extensions.Configuration;
using Radzen;

namespace Churchee.Module.Site.Features.HtmlEditor.Commands.UploadImage
{
    public class UploadImageCommandHandler : IRequestHandler<UploadImageCommand, string>
    {
        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly IConfiguration _configuration;
        private readonly ITenantResolver _tenantResolver;

        public UploadImageCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser, IConfiguration configuration, ITenantResolver tenantResolver)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _configuration = configuration;
            _tenantResolver = tenantResolver;
        }

        public async Task<string> Handle(UploadImageCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string extension = Path.GetExtension(request.File.FileName);

            string fileName = Path.GetFileNameWithoutExtension(request.File.FileName).ToDevName();

            string folderPath = await GetFolderPath(applicationTenantId, cancellationToken);

            string filePath = $"/{folderPath.ToDevName()}{fileName.ToDevName()}{extension}";

            using var ms = request.File.OpenReadStream();

            string finalFilePath = await _blobStore.SaveAsync(applicationTenantId, filePath, ms, true, cancellationToken);

            string urlPrefix = _configuration.GetSection("Images")["Prefix"];

            string domain = urlPrefix.Replace("*", _tenantResolver.GetTenantDevName());

            return $"{domain}{finalFilePath}";
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

            return uploadsFolder.Path.ToLowerInvariant();
        }
    }
}