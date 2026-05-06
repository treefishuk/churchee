using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Reviews.Entities;
using Churchee.Module.Reviews.Specifications;
using MediatR;

namespace Churchee.Module.Reviews.Features.Queries
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
            var repo = _storage.GetRepository<Review>();

            var dataTableResponse = await repo.GetDataTableResponseAsync(
                specification: new ReviewTextFilterSpecification(request.SearchText),
                orderBy: request.OrderBy,
                orderByDir: request.OrderByDirection,
                skip: request.Skip,
                take: request.Take,
                selector: s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    ReviewerImageUrl = s.ReviewerImageUrl,
                    Reviewer = s.ReviewerName,
                    CreatedDate = s.CreatedDate ?? DateTime.Now,
                    Rating = s.Rating
                },
                cancellationToken: cancellationToken);

            return dataTableResponse;
        }
    }
}
