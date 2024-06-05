using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetListingQueryHandler : IRequestHandler<GetListingQuery, IEnumerable<GetListingQueryResponseItem>>
    {

        private readonly IDataStore _store;
        private readonly ICurrentUser _currentUser;

        public GetListingQueryHandler(IDataStore store, ICurrentUser currentUser)
        {
            _store = store;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<GetListingQueryResponseItem>> Handle(GetListingQuery request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            bool isSysAdmin = _currentUser.HasRole("SysAdmin");

            return await _store.GetRepository<ApplicationTenant>()
                .GetQueryable()
                .Where(w => isSysAdmin || w.Id == applicationTenantId)
                .Select(s => new GetListingQueryResponseItem { Id = s.Id, Name = s.Name, CharityNumber = s.CharityNumber })
                .ToListAsync(cancellationToken);
        }
    }
}
