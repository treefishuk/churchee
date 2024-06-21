using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaFoldersQueryHandler : IRequestHandler<GetMediaFoldersQuery, IEnumerable<GetMediaFoldersQueryResponseItem>>
    {

        private readonly IDataStore _storage;

        public GetMediaFoldersQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<GetMediaFoldersQueryResponseItem>> Handle(GetMediaFoldersQuery request, CancellationToken cancellationToken)
        {
            var response = await _storage.GetRepository<MediaFolder>()
                .GetQueryable()
                .Where(w => w.ParentId == request.ParentId)
                .Select(s => new GetMediaFoldersQueryResponseItem(s.Id, s.Name, s.Path, s.Children.Count != 0)).ToListAsync(cancellationToken);

            return response;
        }

    }
}
