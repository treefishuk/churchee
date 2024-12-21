using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Queries.GetPageOfPageTypeContent
{
    public class GetContentTypesForPageTypeQueryHandler : IRequestHandler<GetContentTypesForPageTypeQuery, IEnumerable<GetContentTypesForPageTypeResponse>>
    {
        private readonly IDataStore _storage;

        public GetContentTypesForPageTypeQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<GetContentTypesForPageTypeResponse>> Handle(GetContentTypesForPageTypeQuery request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<PageTypeContent>()
                .GetListAsync(new PageContentTypesForPageType(request.PageTypeId), s => new GetContentTypesForPageTypeResponse
                {
                    Id = s.Id,
                    DevName = s.DevName,
                    Name = s.Name,
                    Required = s.IsRequired,
                    Type = s.Type,
                    Order = s.Order
                }, cancellationToken);
        }
    }
}
