using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetPageDetailsRequestHandler : IRequestHandler<GetPageDetailsRequest, GetPageDetailsResponse>
    {

        private readonly IDataStore _storage;

        public GetPageDetailsRequestHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<GetPageDetailsResponse> Handle(GetPageDetailsRequest request, CancellationToken cancellationToken)
        {

            var result = _storage.GetRepository<Page>().GetQueryable()

                .Where(w => w.Id == request.PageId)
                .Select(s => new GetPageDetailsResponse
                {
                    Title = s.Title,
                    ParentId = s.ParentId,
                    ParentName = s.Parent.Title,
                    Description = s.Description,
                    Url = s.Url,
                    Published = s.Published,
                    Order = s.Order ?? 10,
                    ContentItems = s.PageContent
                    .OrderBy(o => o.PageTypeContent.Order)
                    .Select(m => new GetPageDetailsResponseContentItem
                    {
                        PageTypeContentId = m.PageTypeContentId,
                        Title = m.PageTypeContent.Name,
                        Type = m.PageTypeContent.Type,
                        Value = m.Value,
                        DevName = m.PageTypeContent.DevName
                    })
                }).FirstOrDefault();


            return await Task.FromResult(result);
        }
    }
}
