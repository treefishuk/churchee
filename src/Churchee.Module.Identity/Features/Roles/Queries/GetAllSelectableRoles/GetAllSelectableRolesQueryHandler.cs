using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Specifications;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesQueryHandler : IRequestHandler<GetAllSelectableRolesQuery, IEnumerable<MultiSelectItem>>
    {

        private readonly IDataStore _store;

        public GetAllSelectableRolesQueryHandler(IDataStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<MultiSelectItem>> Handle(GetAllSelectableRolesQuery request, CancellationToken cancellationToken)
        {
            return await _store.GetRepository<ApplicationRole>().GetListAsync(new SelectableRolesSpecification(), s => new MultiSelectItem(s.Id, s.Name), cancellationToken);
        }
    }
}
