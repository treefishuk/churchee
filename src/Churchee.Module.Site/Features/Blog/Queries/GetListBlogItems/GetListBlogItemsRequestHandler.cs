using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Blog.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems
{
    public class GetListBlogItemsRequestHandler : IRequestHandler<GetListBlogItemsRequest, DataTableResponse<GetListBlogItemsResponseItem>>
    {
        private readonly IDataStore _storage;

        public GetListBlogItemsRequestHandler(IDataStore storage)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        public async Task<DataTableResponse<GetListBlogItemsResponseItem>> Handle(GetListBlogItemsRequest request, CancellationToken cancellationToken)
        {

            var repo = _storage.GetRepository<Article>();

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
                .Select(s => new GetListBlogItemsResponseItem
                {
                    Id = s.Id,
                    Title = s.Title,
                    Modified = s.ModifiedDate,
                    Url = s.Url,
                    Published = s.Published
                })
                .ToListAsync(cancellationToken);

            return new DataTableResponse<GetListBlogItemsResponseItem>
            {
                RecordsTotal = count,
                RecordsFiltered = count,
                Draw = request.Take,
                Data = items
            };

        }
    }

}
