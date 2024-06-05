using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Exceptions;
using Churchee.Common.Storage;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Churchee.Module.Facebook.Events.Features.Queries
{
    public class GetAuthUrlQueryHandler : IRequestHandler<GetAuthUrlQuery, string>
    {

        private readonly IConfiguration _configuration;
        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;

        public GetAuthUrlQueryHandler(IConfiguration configuration, ISettingStore settingStore, ICurrentUser currentUser)
        {
            _configuration = configuration;
            _settingStore = settingStore;
            _currentUser = currentUser;
        }

        public async Task<string> Handle(GetAuthUrlQuery request, CancellationToken cancellationToken)
        {
            var settingId = Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68");
            var settingStateId = Guid.Parse("841fb9d0-92ca-41b2-9cdb-5903a6ab7bad");

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(settingId, applicationTenantId, "FacebookPageId", request.PageId);

            string facebookAppId = _configuration.GetValue<string>("facebookAppId") ?? string.Empty;

            if(string.IsNullOrEmpty(facebookAppId))
            {
                throw new MissingConfirgurationSettingException(nameof(facebookAppId));
            }

            string stateId = Guid.NewGuid().ToString();

            await _settingStore.AddOrUpdateSetting(settingStateId, applicationTenantId, "FacebookStateId", stateId);

            return $"https://www.facebook.com/v18.0/dialog/oauth?client_id={facebookAppId}&redirect_uri={request.Domain}/management/integrations/facebook-events/auth?state={stateId}";
        }
    }
}
