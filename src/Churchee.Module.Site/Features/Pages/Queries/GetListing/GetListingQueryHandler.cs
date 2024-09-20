using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQueryHandler : IRequestHandler<GetListingQuery, IEnumerable<GetListingQueryResponseItem>>
    {

        private readonly IDataStore _storage;

        public GetListingQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<GetListingQueryResponseItem>> Handle(GetListingQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Page>();

            int count = repo.Count();

            var query = repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.Title.Contains(request.SearchText));
            }

            if (request.ParentId != null)
            {
                query = query.Where(w => w.ParentId == request.ParentId);
            }

            if (request.ParentId == null)
            {
                query = query.Where(w => w.ParentId == null);
            }

            var items = await query
                .OrderBy(o => o.Url.Length)
                .Select(s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    HasChildren = s.Children.Any(a => a.IsSystem == false),
                    Title = s.Title,
                    Url = s.Url,
                    Created = s.CreatedDate,
                    Modified = s.ModifiedDate,
                    ParentId = s.ParentId,
                    Published = s.Published
                })
                .ToListAsync(cancellationToken);

            return items;

        }
    }
}
