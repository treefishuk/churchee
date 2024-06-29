using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
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
            return await _store.GetRepository<ApplicationRole>().GetQueryable().Where(w => w.Selectable)
                .Select(s => new MultiSelectItem { Value = s.Id, Text = s.Name })
                .ToListAsync(cancellationToken);
        }
    }
}
