using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Templates.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Site.Features.Templates.Queries.GetTemplatesListing
{
    internal class GetTemplatesListingQueryHandler : IRequestHandler<GetTemplatesListingQuery, DataTableResponse<TemplateListingResponse>>
    {

        private readonly IDataStore _storage;

        public GetTemplatesListingQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<DataTableResponse<TemplateListingResponse>> Handle(GetTemplatesListingQuery request, CancellationToken cancellationToken)
        {

            var repo = _storage.GetRepository<ViewTemplate>();

            int count = repo.Count();

            var query = repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.Location.Contains(request.SearchText));
            }

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var items = await query
                .OrderBy(orderby)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(s => new TemplateListingResponse
                {
                    Id = s.Id,
                    HasChildren = 0,
                    Location = s.Location
                })
                .ToListAsync(cancellationToken);

            return new DataTableResponse<TemplateListingResponse>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Draw,
                Data = items
            };
        }
    }
}
