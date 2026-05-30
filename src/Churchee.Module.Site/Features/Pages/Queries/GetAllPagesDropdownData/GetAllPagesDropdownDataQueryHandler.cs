using Churchee.Common.Storage;
using Churchee.CQRS.Abstractions;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.UI.Models;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetAllPagesDropdownDataQueryHandler : IRequestHandler<GetAllPagesDropdownDataQuery, IEnumerable<DropdownInput>>
    {

        private readonly IDataStore _storage;

        public GetAllPagesDropdownDataQueryHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<IEnumerable<DropdownInput>> Handle(GetAllPagesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<WebContent>().GetListAsync(new PagesAndArticlesSpecification(),
                selector: s => new DropdownInput { Title = s.Title, Value = s.Id.ToString() },
                cancellationToken: cancellationToken);

        }
    }
}
