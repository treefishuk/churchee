﻿using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Events.Features.Queries
{
    public class GetListingQueryHandler : IRequestHandler<GetListingQuery, DataTableResponse<GetListingQueryResponseItem>>
    {

        private readonly IDataStore _storage;
        private readonly IConfiguration _configuration;

        public GetListingQueryHandler(IDataStore storage, IConfiguration configuration)
        {
            _storage = storage;
            _configuration = configuration;
        }

        public async Task<DataTableResponse<GetListingQueryResponseItem>> Handle(GetListingQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Event>();

            int count = repo.Count();

            var query = repo.GetQueryable();

            if (!string.IsNullOrEmpty(request.SearchText))
            {
                query = query.Where(w => w.Title.Contains(request.SearchText));
            }

            string orderby = $"{request.OrderBy} {request.OrderByDirection}";

            var items = await query
                .OrderBy(orderby)
                .Skip(request.Skip)
                .Take(request.Take)
                .Select(s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    Active = true,
                    ImageUri = s.ImageUrl,
                    Title = s.Title,
                    CreatedDate = s.CreatedDate ?? DateTime.Now,
                    Source = s.SourceName
                })
                .ToListAsync(cancellationToken);

            return new DataTableResponse<GetListingQueryResponseItem>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Draw,
                Data = items
            };

        }
    }
}
