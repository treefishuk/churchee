using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Events.Specifications;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesForUserQueryHandler : IRequestHandler<GetAllSelectableRolesForUserQuery, IEnumerable<MultiSelectItem>>
    {

        private readonly IDataStore _store;
        private readonly ChurcheeUserManager _churcheeUserManager;

        public GetAllSelectableRolesForUserQueryHandler(IDataStore store, ChurcheeUserManager churcheeUserManager)
        {
            _store = store;
            _churcheeUserManager = churcheeUserManager;
        }

        public async Task<IEnumerable<MultiSelectItem>> Handle(GetAllSelectableRolesForUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _churcheeUserManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            var userRoles = await _churcheeUserManager.GetRolesAsync(user);

            var data = await _store.GetRepository<ApplicationRole>().GetListAsync(new SelectableRolesSpecification(), s => new MultiSelectItem(s.Id, s.Name, userRoles.Any(a => a == s.Name)), cancellationToken);

            return data;
        }
    }
}
