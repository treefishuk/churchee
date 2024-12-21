using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Media.Commands
{
    public class DeleteMediaFolderCommandHandler : IRequestHandler<DeleteMediaFolderCommand, CommandResponse>
    {

        private readonly ICurrentUser _currentUser;
        private readonly IDataStore _dataStore;

        public DeleteMediaFolderCommandHandler(ICurrentUser currentUser, IDataStore dataStore)
        {
            _currentUser = currentUser;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(DeleteMediaFolderCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                var repo = _dataStore.GetRepository<MediaFolder>();

                await repo.SoftDelete(request.FolderId);

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
            catch (Exception)
            {
                response.AddError("Failed to remove folder", "folderId");
            }

            return new CommandResponse();
        }
    }
}
