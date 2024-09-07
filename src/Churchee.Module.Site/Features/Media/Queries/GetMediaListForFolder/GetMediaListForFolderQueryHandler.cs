using Churchee.Common.Storage;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Media.Queries
{
    public class GetMediaListForFolderQueryHandler : IRequestHandler<GetMediaListForFolderQuery, IEnumerable<GetMediaListForFolderQueryResponseItem>>
    {

        private readonly IDataStore _storage;

        public GetMediaListForFolderQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<GetMediaListForFolderQueryResponseItem>> Handle(GetMediaListForFolderQuery request, CancellationToken cancellationToken)
        {
            var response = await _storage.GetRepository<Entities.MediaItem>()
                .GetQueryable()
                .Where(w => w.MediaFolderId == request.MediaFolderId)
                .Select(s => new GetMediaListForFolderQueryResponseItem(s.Id, s.Title, s.Description, s.Html, s.MediaUrl, s.LinkUrl, s.CssClass)).ToListAsync(cancellationToken);

            return response;

        }
    }
}
