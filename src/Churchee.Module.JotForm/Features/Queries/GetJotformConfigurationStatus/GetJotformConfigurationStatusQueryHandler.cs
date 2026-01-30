using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using MediatR;

namespace Churchee.Module.Jotform.Features.Queries.GetJotformConfigurationStatus
{
    public class GetJotformConfigurationStatusQueryHandler : IRequestHandler<GetJotformConfigurationStatusQuery, GetJotformConfigurationStatusResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public GetJotformConfigurationStatusQueryHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<GetJotformConfigurationStatusResponse> Handle(GetJotformConfigurationStatusQuery request, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Token>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var token = await repo.FirstOrDefaultAsync(new GetTokenByKeySpecification("Jotform", applicationTenantId), cancellationToken);

            return token == null
                ? new GetJotformConfigurationStatusResponse
                {
                    Configured = false
                }
                : new GetJotformConfigurationStatusResponse
                {
                    Configured = true
                };
        }
    }
}
