using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Helpers;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Queries
{
    public class GetOAuthRedirectQueryHandler : IRequestHandler<GetOAuthRedirectQuery, string>
    {
        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;

        public GetOAuthRedirectQueryHandler(ISettingStore settingStore, ICurrentUser currentUser)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
        }

        public async Task<string> Handle(GetOAuthRedirectQuery request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(SettingKeys.ClientId, applicationTenantId, "API Client Id", request.ClientId);
            await _settingStore.AddOrUpdateSetting(SettingKeys.ClientSecret, applicationTenantId, "API Client Secret", request.ClientSecret);
            await _settingStore.AddOrUpdateSetting(SettingKeys.BusinessProfileId, applicationTenantId, "Profile ID", request.BusinessProfileId);

            string redirectUri = $"{request.Domain}/management/integrations/google-reviews/auth";

            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = request.ClientId,
                        ClientSecret = request.ClientSecret
                    },
                    Scopes =
                    [
                        "https://www.googleapis.com/auth/business.manage"
                    ],
                    Prompt = "consent"
                });

            return flow.CreateAuthorizationCodeRequest(redirectUri).Build().ToString();
        }
    }
}
