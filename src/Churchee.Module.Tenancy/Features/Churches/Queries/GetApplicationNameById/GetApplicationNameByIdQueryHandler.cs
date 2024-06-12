using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetApplicationNameByIdQueryHandler : IRequestHandler<GetApplicationNameByIdQuery, string>
    {
        private readonly IDataStore _dataStore;

        public GetApplicationNameByIdQueryHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<string> Handle(GetApplicationNameByIdQuery request, CancellationToken cancellationToken)
        {
            return await _dataStore.GetRepository<ApplicationTenant>().GetQueryable().Where(w => w.Id == request.ApplicationTenantId).Select(s => s.DevName).FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
        }
    }
}
