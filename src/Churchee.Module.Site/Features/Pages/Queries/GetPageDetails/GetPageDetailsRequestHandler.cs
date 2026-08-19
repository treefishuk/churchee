using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;

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

            var result = await _storage.GetRepository<Page>().FirstOrDefaultAsync(new PageWithContentAndPropertiesSpecification(request.PageId), s => new GetPageDetailsResponse
            {
                Title = s.Title,
                ParentId = s.ParentId,
                ParentName = s.Parent.Title,
                Description = s.Description,
                Url = s.Url,
                Published = s.Published,
                Order = s.Order,
                ImageThumbnail = string.IsNullOrEmpty(s.ImageUrl) ? "/_content/Churchee.Module.UI/img/opengraph-placeholder.png" : s.ImageUrl + "_t.webp",
                ContentItems = s.PageContent
                    .OrderBy(o => o.PageTypeContent.Order)
                    .Select(m => new GetPageDetailsResponseContentItem
                    {
                        PageTypeContentId = m.PageTypeContentId,
                        Title = m.PageTypeContent.Name,
                        Type = m.PageTypeContent.Type,
                        Value = m.Value,
                        DevName = m.PageTypeContent.DevName,
                        MaxLength = m.PageTypeContent.MaxLength,
                    })
            }, cancellationToken);


            return result;
        }
    }
}
