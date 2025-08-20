using Churchee.Common.Abstractions;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Specifications;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Churchee.Module.Events.Features.Queries
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
            var repo = _storage.GetRepository<Event>();

            var now = DateTime.Now;

            var dataTableResponse = await repo.GetDataTableResponseAsync(
                specification: new EventTextFilterSpecification(request.SearchText),
                orderBy: request.OrderBy,
                orderByDir: request.OrderByDirection,
                skip: request.Skip,
                take: request.Take,
                selector: s => new GetListingQueryResponseItem
                {
                    Id = s.Id,
                    Active = true,
                    ImageUri = s.ImageUrl,
                    Title = s.Title,
                    CreatedDate = s.CreatedDate ?? DateTime.Now,
                    NextDate = s.EventDates.FirstOrDefault(w => w.Start > now).Start,
                    Source = s.SourceName
                },
                cancellationToken: cancellationToken);

            ChangeImagesToThumbnailImages(dataTableResponse);

            return dataTableResponse;
        }

        private static void ChangeImagesToThumbnailImages(DataTableResponse<GetListingQueryResponseItem> dataTableResponse)
        {
            foreach (var item in dataTableResponse.Data.Where(w => !string.IsNullOrEmpty(w.ImageUri)))
            {
                string fileName = Path.GetFileNameWithoutExtension(item.ImageUri);

                item.ImageUri = item.ImageUri.Replace(fileName, $"{fileName}_t");
            }
        }
    }
}
