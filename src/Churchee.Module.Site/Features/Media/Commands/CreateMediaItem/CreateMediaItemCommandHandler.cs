using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using Churchee.Module.Tenancy.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaItemCommandHandler : IRequestHandler<CreateMediaItemCommand, CommandResponse>
    {
        private readonly IBlobStore _blobStore;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public CreateMediaItemCommandHandler(IBlobStore blobStore, IDataStore dataStore, ICurrentUser currentUser)
        {
            _blobStore = blobStore;
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreateMediaItemCommand request, CancellationToken cancellationToken)
        {

            string folderPath = _dataStore.GetRepository<MediaFolder>().GetQueryable().Where(w => w.Id == request.FolderId).Select(s => s.Path).FirstOrDefault() ?? string.Empty;  

            byte[] data = Convert.FromBase64String(request.Base64Image.Split(',')[1]);

            using var ms = new MemoryStream(data);

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string imagePath = $"/img/{folderPath.ToDevName()}{request.Name.ToDevName()}{request.Extention}";

            await _blobStore.SaveAsync(applicationTenantId, imagePath, ms, true, cancellationToken);

            var media = new MediaItem(applicationTenantId, request.Name, imagePath, request.Description, request.AdditionalContent, request.FolderId, request.LinkUrl);

            _dataStore.GetRepository<MediaItem>().Create(media);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
