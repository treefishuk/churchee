using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Exceptions;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.API;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly JsonSerializerOptions _options;
        private readonly ILogger _logger;

        public EnableFacebookIntegrationCommandHandler(IHttpClientFactory clientFactory, ICurrentUser currentUser, ISettingStore settingStore, IConfiguration configuration, IDataStore dataStore, ILogger<EnableFacebookIntegrationCommandHandler> logger)
        {
            _clientFactory = clientFactory;
            _currentUser = currentUser;
            _settingStore = settingStore;
            _configuration = configuration;
            _dataStore = dataStore;
            _options = GetOptions();
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(EnableFacebookIntegrationCommand request, CancellationToken cancellationToken)
        {
            var tokenRepo = _dataStore.GetRepository<Token>();
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string facebookAppId = GetConfigurationValue("facebookAppId");
            string appSecret = GetConfigurationValue("facebookAppSecret");
            string pageId = await GetSettingValue("3de048ae-d711-4609-9b66-97564a9d0d68", applicationTenantId);
            string stateId = await GetSettingValue("841fb9d0-92ca-41b2-9cdb-5903a6ab7bad", applicationTenantId);
            string redirectUri = $"{request.Domain}/management/integrations/facebook-events/auth?state={stateId}";

            var client = CreateClient();

            if (client == null)
            {
                var response = new CommandResponse();

                response.AddError("Missing Facebook API Url Setting", "");

                return response;
            }

            var accessToken = await GetAccessToken(facebookAppId, appSecret, stateId, redirectUri, request.Token, client, cancellationToken);

            if (accessToken == null)
            {
                return new CommandResponse();
            }

            StoreAccessToken(tokenRepo, applicationTenantId, accessToken);

            var userIdResponse = await GetFacebookUserId(client, accessToken, cancellationToken);

            if (userIdResponse == null)
            {
                return new CommandResponse();
            }

            await StoreFacebookUserId(applicationTenantId, userIdResponse);

            var pageTokensResponse = await GetAvailablePageTokensForUser(client, accessToken, userIdResponse, cancellationToken);

            if (pageTokensResponse == null)
            {
                return new CommandResponse();
            }

            string pageToken = GetPageAccessTokenForPage(pageId, pageTokensResponse);

            StorePageAccessToken(tokenRepo, applicationTenantId, pageToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private static string GetPageAccessTokenForPage(string pageId, FacebookPagesReponse pageTokensResponse)
        {
            return pageTokensResponse.Data.Where(w => w.Id == pageId).Select(s => s.AccessToken).FirstOrDefault() ?? string.Empty;
        }

        private static void StorePageAccessToken(IRepository<Token> tokenRepo, Guid applicationTenantId, string pageToken)
        {
            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.FacebookPageAccessToken, pageToken));
        }

        private async Task<FacebookPagesReponse> GetAvailablePageTokensForUser(HttpClient client, FacebookAccessTokenResponse accessToken, FacebookUserIdResponse userIdResponse, CancellationToken cancellationToken)
        {
            string pagesResponseJson = await client.GetStringAsync($"{userIdResponse.Id}/accounts?access_token={accessToken.AccessToken}", cancellationToken);

            var pagesResponse = JsonSerializer.Deserialize<FacebookPagesReponse>(pagesResponseJson, _options);

            return pagesResponse;
        }

        private async Task StoreFacebookUserId(Guid applicationTenantId, FacebookUserIdResponse userIdResponse)
        {
            await _settingStore.AddOrUpdateSetting(Guid.Parse("efa5252a-e825-4310-a498-2383875e9584"), applicationTenantId, "FacebookUserId", userIdResponse.Id);
        }

        private static void StoreAccessToken(IRepository<Token> tokenRepo, Guid applicationTenantId, FacebookAccessTokenResponse accessToken)
        {
            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.FacebookAccessToken, accessToken.AccessToken));
        }

        private async Task<FacebookUserIdResponse> GetFacebookUserId(HttpClient client, FacebookAccessTokenResponse accessToken, CancellationToken cancellationToken)
        {
            var json = await client.GetStringAsync($"me?fields=id&access_token={accessToken.AccessToken}", cancellationToken);

            return JsonSerializer.Deserialize<FacebookUserIdResponse>(json, _options);
        }

        private async Task<FacebookAccessTokenResponse> GetAccessToken(string facebookAppId, string appSecret, string stateId, string redirectUri, string code, HttpClient client, CancellationToken cancellationToken)
        {
            string jsonString = await client.GetStringAsync($"oauth/access_token?client_id={facebookAppId}&redirect_uri={redirectUri}&client_secret={appSecret}&code={code}&state={stateId}", cancellationToken);

            if (string.IsNullOrEmpty(jsonString))
            {
                return null;
            }

            return JsonSerializer.Deserialize<FacebookAccessTokenResponse>(jsonString, _options);
        }

        private HttpClient CreateClient()
        {
            var facebookApiUrl = _configuration.GetSection("Facebook").GetValue<string>("Api");

            if (string.IsNullOrEmpty(facebookApiUrl))
            {
                _logger.LogError("Missing Facebook API Setting");

                return null;
            }

            var client = _clientFactory.CreateClient();

            client.BaseAddress = new Uri(facebookApiUrl);

            return client;
        }

        private string GetConfigurationValue(string key)
        {
            string value = _configuration.GetValue<string>(key) ?? string.Empty;

            if (string.IsNullOrEmpty(value))
            {
                throw new MissingConfirgurationSettingException(key);
            }

            return value;
        }

        private async Task<string> GetSettingValue(string settingId, Guid tenantId)
        {
            return await _settingStore.GetSettingValue(Guid.Parse(settingId), tenantId);
        }

        private static JsonSerializerOptions GetOptions()
        {
            var options = new JsonSerializerOptions();

            options.Converters.Add(new DateTimeIso8601JsonConverter());

            return options;
        }
    }
}