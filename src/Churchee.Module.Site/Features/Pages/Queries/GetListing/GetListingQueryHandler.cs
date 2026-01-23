using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetListingQueryHandler : IRequestHandler<GetListingQuery, DataTableResponse<GetListingQueryResponseItem>>
    {

        private readonly IDataStore _storage;

        public GetListingQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<DataTableResponse<GetListingQueryResponseItem>> Handle(GetListingQuery request, CancellationToken cancellationToken)
        {
            var repo = _storage.GetRepository<Page>();

            return await repo.GetDataTableResponseAsync(
                specification: new PageListingSpecification(request.SearchText, request.ParentId),
                orderBy: request.OrderBy,
                orderByDir: request.OrderByDirection,
                skip: request.Skip,
                take: request.Take,
                selector: s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    HasChildren = s.Children.Any(a => !a.IsSystem),
                    Title = s.Title,
                    Url = s.Url,
                    Modified = s.ModifiedDate,
                    ParentId = s.ParentId,
                    Published = s.Published
                },
                cancellationToken: cancellationToken);
        }
    }
}
