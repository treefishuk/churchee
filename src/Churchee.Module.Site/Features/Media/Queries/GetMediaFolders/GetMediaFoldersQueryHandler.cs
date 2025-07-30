using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaFoldersQueryHandler : IRequestHandler<GetMediaFoldersQuery, IEnumerable<GetMediaFoldersQueryResponseItem>>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public GetMediaFoldersQueryHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GetMediaFoldersQueryResponseItem>> Handle(GetMediaFoldersQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<MediaFolder>();

            await CreateDefaultRootFoldersIfMissing(request, cancellationToken);

            var response = await repo
                .GetQueryable()
                .Where(w => w.ParentId == request.ParentId)
                .Select(s => new GetMediaFoldersQueryResponseItem(s.Id, s.Name, s.Path, s.Children.Count != 0, s.SupportedFileTypes)).ToListAsync(cancellationToken);

            return response;
        }

        private async Task CreateDefaultRootFoldersIfMissing(GetMediaFoldersQuery request, CancellationToken cancellationToken)
        {
            if (request.ParentId != null)
            {
                return;
            }

            var repo = _storage.GetRepository<MediaFolder>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            if (!repo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.ParentId == null && a.Path == "Documents/"))
            {
                repo.Create(new MediaFolder(applicationTenantId, "Documents", ".pdf"));
            }

            if (!repo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.ParentId == null && a.Path == "Audio/"))
            {
                repo.Create(new MediaFolder(applicationTenantId, "Audio", ".mp3"));
            }

            if (!repo.AnyWithFiltersDisabled(a => a.ApplicationTenantId == applicationTenantId && a.ParentId == null && a.Path == "Images/"))
            {
                repo.Create(new MediaFolder(applicationTenantId, "Images", ".jpg, .jpeg, .png, .webp"));
            }

            await _storage.SaveChangesAsync(cancellationToken);

        }
    }
}
