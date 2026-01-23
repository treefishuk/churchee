using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Videos.Entities;
using Churchee.Module.Videos.Specifications;
using MediatR;

namespace Churchee.Module.Videos.Features.Queries
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
            var repo = _storage.GetRepository<Video>();

            var dataTableResponse = await repo.GetDataTableResponseAsync(
                specification: new VideoSearchFilterSpecification(request.SearchText),
                orderBy: request.OrderBy,
                orderByDir: request.OrderByDirection,
                skip: request.Skip,
                take: request.Take,
                selector: s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    Active = true,
                    ImageUri = s.ThumbnailUrl,
                    Title = s.Title,
                    PublishedDate = s.PublishedDate,
                    Source = s.SourceName
                },
                cancellationToken: cancellationToken);

            return dataTableResponse;
        }
    }
}
