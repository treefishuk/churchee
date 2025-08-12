using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Site.Features.Redirects.Queries
{
    public class GetListOfRedirectsQueryHandler : IRequestHandler<GetListOfRedirectsQuery, DataTableResponse<GetListOfRedirectsResponseItem>>
    {
        private readonly IDataStore _storage;

        public GetListOfRedirectsQueryHandler(IDataStore storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<DataTableResponse<GetListOfRedirectsResponseItem>> Handle(GetListOfRedirectsQuery request, CancellationToken cancellationToken)
        {

            var repo = _storage.GetRepository<RedirectUrl>();

            int count = repo.Count();

            var query = repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.Path.Contains(request.SearchText));
            }

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var items = await query
                .OrderBy(orderby)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(s => new GetListOfRedirectsResponseItem
                {
                    Id = s.Id,
                    Path = s.Path,
                    RedirectsTo = s.WebContent.Title
                })
                .ToListAsync(cancellationToken);

            return new DataTableResponse<GetListOfRedirectsResponseItem>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Take,
                Data = items
            };

        }
    }

}
