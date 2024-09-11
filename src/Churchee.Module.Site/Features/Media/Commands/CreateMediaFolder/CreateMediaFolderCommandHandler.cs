using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class CreateMediaFolderCommandHandler : IRequestHandler<CreateMediaFolderCommand, CommandResponse>
    {

        private readonly ICurrentUser _currentUser;
        private readonly IDataStore _dataStore;

        public CreateMediaFolderCommandHandler(ICurrentUser currentUser, IDataStore dataStore)
        {
            _currentUser = currentUser;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(CreateMediaFolderCommand request, CancellationToken cancellationToken)
        {

            var repo = _dataStore.GetRepository<MediaFolder>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var parent = repo.GetById(request.ParentId);

            repo.Create(new MediaFolder(applicationTenantId, request.Name, parent));

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
