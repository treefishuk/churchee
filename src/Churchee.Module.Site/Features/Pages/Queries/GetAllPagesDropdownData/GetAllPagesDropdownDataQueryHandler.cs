using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Specifications;
using Churchee.Module.UI.Models;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Queries
{
    public class GetAllPagesDropdownDataQueryHandler : IRequestHandler<GetAllPagesDropdownDataQuery, IEnumerable<DropdownInput>>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public GetAllPagesDropdownDataQueryHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<DropdownInput>> Handle(GetAllPagesDropdownDataQuery request, CancellationToken cancellationToken)
        {
            return await _storage.GetRepository<Page>().GetListAsync(new AllPagesSpecification(),
                selector: s => new DropdownInput { Title = s.Title, Value = s.Id.ToString() },
                cancellationToken: cancellationToken);

        }
    }
}
