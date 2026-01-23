using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.UI.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetParentArticlePagesDropdownDataQueryHandler : IRequestHandler<GetParentArticlePagesDropdownDataQuery, IEnumerable<DropdownInput>>
    {
        private readonly IDataStore _storage;

        public GetParentArticlePagesDropdownDataQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<DropdownInput>> Handle(GetParentArticlePagesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<Page>()
                .GetQueryable()
                .Where(w => w.PageType.SystemKey == Helpers.PageTypes.BlogListingPageTypeId)
                .Select(s => new DropdownInput { Title = s.Title, Value = s.Id.ToString() })
                .ToListAsync(cancellationToken);
        }
    }
}
