using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Google.Reviews.Jobs;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Hangfire;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class EnableGoogleReviewsIntegrationCommandHandler : IRequestHandler<EnableGoogleReviewsIntegrationCommand, CommandResponse>
    {
        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;
        private readonly IDataStore _dataStore;
        private readonly IJobService _jobShedularService;
        public IAuthorizationCodeFlow Flow { get; set; }

        public EnableGoogleReviewsIntegrationCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IDataStore dataStore, IJobService jobShedularService, IAuthorizationCodeFlow authorizationFlow = null)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _dataStore = dataStore;
            _jobShedularService = jobShedularService;

            if (authorizationFlow != null)
            {
                Flow = authorizationFlow;
            }
        }

        public async Task<CommandResponse> Handle(EnableGoogleReviewsIntegrationCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string userId = _currentUser.GetUserId();
            string clientId = await _settingStore.GetSettingValue(SettingKeys.ClientId, applicationTenantId);
            string clientSecret = await _settingStore.GetSettingValue(SettingKeys.ClientSecret, applicationTenantId);

            string redirectUri = $"{request.Domain}/management/integrations/google-reviews/auth";

            if (Flow == null)
            {
                Flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = ["https://www.googleapis.com/auth/business.manage"]
                });
            }

            var tokenResponse = await Flow.ExchangeCodeForTokenAsync(
                userId: userId,
                code: request.Code,
                redirectUri: redirectUri,
                taskCancellationToken: cancellationToken);

            var tokenRepo = _dataStore.GetRepository<Token>();

            await StoreToken(tokenRepo, applicationTenantId, SettingKeys.GoogleReviewsAccessTokenKey.ToString(), tokenResponse.AccessToken, cancellationToken);

            await StoreToken(tokenRepo, applicationTenantId, SettingKeys.GoogleReviewsRefreshTokenKey.ToString(), tokenResponse.RefreshToken, cancellationToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            _jobShedularService.ScheduleJob<GoogleReviewsSyncJob>($"{applicationTenantId}_GoogleReviews", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Daily);

            _jobShedularService.QueueJob<GoogleReviewsSyncJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));

            return new CommandResponse();
        }

        private static async Task StoreToken(IRepository<Token> tokenRepo, Guid applicationTenantId, string key, string value, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            var existingToken = await tokenRepo.FirstOrDefaultAsync(new GetTokenByKeySpecification(key, applicationTenantId), cancellationToken);

            if (existingToken != null)
            {
                existingToken.UpdateValue(value);
                tokenRepo.Update(existingToken);
                return;
            }

            tokenRepo.Create(new Token(applicationTenantId, key, value));
        }
    }
}
