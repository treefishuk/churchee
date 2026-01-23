using Ardalis.Specification;
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

            var redirectQuery = _storage.GetRepository<RedirectUrl>().GetQueryable();
            var webContentQuery = _storage.GetRepository<WebContent>().GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                redirectQuery = redirectQuery.Where(w => w.Path.Contains(request.SearchText));
            }

            var joinedQuery = from r in redirectQuery
                              join wc in webContentQuery on r.WebContentId equals wc.Id
                              select new GetListOfRedirectsResponseItem
                              {
                                  Id = r.Id,
                                  Path = r.Path,
                                  RedirectsTo = wc.Title,
                              };

            int count = await joinedQuery.CountAsync(cancellationToken: cancellationToken);

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var data = await joinedQuery
                .OrderBy(orderby)
                .Skip(request.Skip)
                .Take(request.Take)
                .ToListAsync(cancellationToken);

            return new DataTableResponse<GetListOfRedirectsResponseItem>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Take,
                Data = data
            };
        }
    }

}
