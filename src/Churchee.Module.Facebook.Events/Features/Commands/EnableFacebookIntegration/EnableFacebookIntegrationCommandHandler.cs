using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Exceptions;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.API;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Churchee.Module.Facebook.Events.Features.Commands
{
    public class EnableFacebookIntegrationCommandHandler : IRequestHandler<EnableFacebookIntegrationCommand, CommandResponse>
    {

        private readonly IHttpClientFactory _clientFactory;
        private readonly ICurrentUser _currentUser;
        private readonly IDataStore _dataStore;
        private readonly ISettingStore _settingStore;
        private readonly IConfiguration _configuration;

        public EnableFacebookIntegrationCommandHandler(IHttpClientFactory clientFactory, ICurrentUser currentUser, ISettingStore settingStore, IConfiguration configuration, IDataStore dataStore)
        {
            _clientFactory = clientFactory;
            _currentUser = currentUser;
            _settingStore = settingStore;
            _configuration = configuration;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(EnableFacebookIntegrationCommand request, CancellationToken cancellationToken)
        {
            var tokenRepo = _dataStore.GetRepository<Token>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string code = request.Token;

            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri("https://graph.facebook.com/v18.0/");

            string facebookAppId = _configuration.GetValue<string>("facebookAppId") ?? string.Empty;

            if (string.IsNullOrEmpty(facebookAppId))
            {
                throw new MissingConfirgurationSettingException(nameof(facebookAppId));
            }

            string pageId = await _settingStore.GetSettingValue(Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68"), applicationTenantId);

            string appSecret = _configuration.GetValue<string>("facebookAppSecret") ?? string.Empty;

            if (string.IsNullOrEmpty(appSecret))
            {
                throw new MissingConfirgurationSettingException(nameof(appSecret));
            }

            string stateId = await _settingStore.GetSettingValue(Guid.Parse("841fb9d0-92ca-41b2-9cdb-5903a6ab7bad"), applicationTenantId);

            string redirectUri = $"{request.Domain}/management/integrations/facebook-events/auth?state={stateId}";

            string jsonString = await client.GetStringAsync($"oauth/access_token?client_id={facebookAppId}&redirect_uri={redirectUri}&client_secret={appSecret}&code={code}&state={stateId}");

            if (string.IsNullOrEmpty(jsonString))
            {
                return new CommandResponse();
            }

            var options = new JsonSerializerOptions();

            options.Converters.Add(new DateTimeISO8601JsonConverter());

            var accessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(jsonString, options);

            if (accessTokenResponse == null)
            {
                return new CommandResponse();
            }

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.FacebookAccessToken, accessTokenResponse.AccessToken));

            string userIdjsonString = await client.GetStringAsync($"me?fields=id&access_token={accessTokenResponse.AccessToken}");

            var userIdResponse = JsonSerializer.Deserialize<FacebookUserIdResponse>(userIdjsonString, options);

            if (userIdResponse == null)
            {
                return new CommandResponse();
            }

            await _settingStore.AddOrUpdateSetting(Guid.Parse("efa5252a-e825-4310-a498-2383875e9584"), applicationTenantId, "FacebookUserId", userIdResponse.Id);

            string pagessResponseJson = await client.GetStringAsync($"{userIdResponse.Id}/accounts?access_token={accessTokenResponse.AccessToken}");

            var pagessResponse = JsonSerializer.Deserialize<FacebookPagesReponse>(pagessResponseJson, options);

            if (pagessResponse == null)
            {
                return new CommandResponse();
            }

            string pageToken = pagessResponse.Data.Where(w => w.Id == pageId).Select(s => s.AccessToken).FirstOrDefault() ?? string.Empty;

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.FacebookPageAccessToken, pageToken));

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();

        }

    }
}
