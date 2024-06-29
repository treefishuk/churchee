using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Features.Contributors.Queries
{
    public class ListContributorsForTenantQueryHandler : IRequestHandler<ListContributorsForTenantQuery, IEnumerable<ListContributorsForTenantResponseItem>>
    {

        private readonly IDataStore _store;
        private readonly ICurrentUser _currentUser;

        public ListContributorsForTenantQueryHandler(IDataStore store, ICurrentUser currentUser)
        {
            _store = store;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<ListContributorsForTenantResponseItem>> Handle(ListContributorsForTenantQuery request, CancellationToken cancellationToken)
        {
            Guid applicationTenantId = await _currentUser.GetApplicationTenantId();

            var items = _store.GetRepository<ApplicationUser>()
                .GetQueryable()
                .Where(w => w.ApplicationTenantId == applicationTenantId)
                .Select(s => new ListContributorsForTenantResponseItem
                {
                    Id = s.Id,
                    UserName = s.UserName,
                    LockedOut = s.LockoutEnd > DateTime.Now
                });

            return items;
        }
    }
}
