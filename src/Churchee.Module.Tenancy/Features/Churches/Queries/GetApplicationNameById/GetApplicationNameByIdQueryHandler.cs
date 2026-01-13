using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Specifications;
using MediatR;

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
            var specification = new ApplicationTenantByIdSpecification(request.ApplicationTenantId);
            var repo = _dataStore.GetRepository<ApplicationTenant>();
            return await repo.FirstOrDefaultAsync(specification, selector => selector.DevName, cancellationToken);
        }
    }
}
