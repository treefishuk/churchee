using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class UpdateMediaItemCommandHandler : IRequestHandler<UpdateMediaItemCommand, CommandResponse>
    {

        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public UpdateMediaItemCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
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

            if (!string.IsNullOrEmpty(request.Base64Image))
            {
                string folderPath = _dataStore.GetRepository<MediaFolder>().GetQueryable().Where(w => w.Id == entity.MediaFolderId).Select(s => s.Path).FirstOrDefault() ?? string.Empty;

                byte[] data = Convert.FromBase64String(request.Base64Image.Split(',')[1]);

                using var ms = new MemoryStream(data);

                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                string imagePath = $"/img/{folderPath.ToDevName()}{request.Name.ToDevName()}{request.Extention}";

                await _blobStore.SaveAsync(applicationTenantId, imagePath, ms, true, false, cancellationToken);

                entity.UpdateMediaUrl(imagePath);
            }


            entity.UpdateDetails(request.Name, request.Description, request.AdditionalContent, request.LinkUrl);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return response;

        }
    }
}
