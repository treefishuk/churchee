using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Site.Features.PageTypes.Queries
{
    internal class GetPageTypesListingQueryHandler : IRequestHandler<GetPageTypesListingQuery, DataTableResponse<GetPageTypesListingResponse>>
    {

        private readonly IDataStore _storage;

        public GetPageTypesListingQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<DataTableResponse<GetPageTypesListingResponse>> Handle(GetPageTypesListingQuery request, CancellationToken cancellationToken)
        {

            var repo = _storage.GetRepository<PageType>();

            int count = repo.Count();

            var query = repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.Name.Contains(request.SearchText));
            }

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var items = await query
                .OrderBy(orderby)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(s => new GetPageTypesListingResponse
                {
                    Id = s.Id,
                    Name = s.Name,
                    DevName = s.DevName,
                    AllowInRoot = s.AllowInRoot
                })
                .ToListAsync(cancellationToken);

            return new DataTableResponse<GetPageTypesListingResponse>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Draw,
                Data = items
            };
        }
    }
}
