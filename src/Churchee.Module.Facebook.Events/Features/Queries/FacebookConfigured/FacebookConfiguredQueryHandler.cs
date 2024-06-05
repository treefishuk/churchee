using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using MediatR;

namespace Churchee.Module.Facebook.Events.Features.Queries
{
    public class FacebookConfiguredQueryHandler : IRequestHandler<FacebookConfiguredQuery, bool>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public FacebookConfiguredQueryHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(FacebookConfiguredQuery request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var tokenRepo = _dataStore.GetRepository<Token>();

            string facebookAccessToken = tokenRepo.ApplySpecification(new GetTokenByKeySpecification(SettingKeys.FacebookAccessToken, applicationTenantId)).Select(s => s.Value).FirstOrDefault();

            return !string.IsNullOrEmpty(facebookAccessToken);
        }
    }
}
