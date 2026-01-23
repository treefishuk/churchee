using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.x.Helpers;
using Churchee.Module.X.Jobs;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync
{
    public class EnableTweetsSyncCommandHandler : IRequestHandler<EnableTweetsSyncCommand, CommandResponse>
    {

        private readonly IJobService _jobService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;
        private readonly ISettingStore _settingStore;
        private readonly ILogger _logger;
        public EnableTweetsSyncCommandHandler(IJobService jobService, IHttpClientFactory httpClientFactory, IDataStore dataStore, ICurrentUser currentUser, ISettingStore settingStore, ILogger<EnableTweetsSyncCommandHandler> logger)
        {
            _jobService = jobService;
            _httpClientFactory = httpClientFactory;
            _dataStore = dataStore;
            _currentUser = currentUser;
            _settingStore = settingStore;
            _logger = logger;
        }

        public async Task<CommandResponse> Handle(EnableTweetsSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var tokenRepo = _dataStore.GetRepository<Token>();

            tokenRepo.Create(new Token(applicationTenantId, SettingKeys.XBearerToken, request.BearerToken));

            await _dataStore.SaveChangesAsync(cancellationToken);

            string accountName = request.AccountName;

            if (accountName.StartsWith('@'))
            {
                accountName = accountName[1..];
            }

            await _settingStore.AddOrUpdateSetting(Guid.Parse(SettingKeys.XUserAccount), applicationTenantId, "X/Twitter UserName", accountName);

            var response = await StoreUserId(accountName, request.BearerToken, applicationTenantId, cancellationToken);

            if (!response.IsSuccess)
            {
                return response;
            }

            string newTemplatePath = "/Views/Shared/Components/Carousel/Tweets.cshtml";

            if (!_dataStore.GetRepository<ViewTemplate>().AnyWithFiltersDisabled(a => a.Location == newTemplatePath && a.ApplicationTenantId == applicationTenantId))
            {
                _dataStore.GetRepository<ViewTemplate>().Create(new ViewTemplate(applicationTenantId, newTemplatePath, ViewTemplateData.TweetListing));

                await _dataStore.SaveChangesAsync(cancellationToken);
            }
            try
            {
                _jobService.ScheduleJob<SyncTweets>($"{applicationTenantId}_SyncTweets", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Hourly);
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Failed to schedule job for syncing tweets. Exception: {Exception}", ex.Message);
                }

                response.AddError("Failed to schedule job for syncing tweets", "");

                return response;
            }

            try
            {
                _jobService.QueueJob<SyncTweets>(x => x.ExecuteAsync(applicationTenantId, CancellationToken.None));
            }
            catch (Exception ex)
            {

                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Failed to queue job for syncing tweets. Exception: {Exception}", ex.Message);
                }

                response.AddError("Failed to queue job for syncing tweets", "");

                return response;
            }

            return response;
        }

        public async Task<CommandResponse> StoreUserId(string accountName, string bearerToken, Guid applicationTenantId, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            using var httpClient = _httpClientFactory.CreateClient();

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            string getUserIdUrl = $"https://api.twitter.com/2/users/by/username/{accountName}";

            var getUserIdResponse = await httpClient.GetAsync(getUserIdUrl, cancellationToken);

            if (!getUserIdResponse.IsSuccessStatusCode)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError("Failed to get user ID from Twitter API. Status code: {StatusCode}", getUserIdResponse.StatusCode);
                }

                response.AddError("Failed to get user ID from Twitter API", "");

                return response;
            }

            string getUserIdResponseString = await getUserIdResponse.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(getUserIdResponseString))
            {
                _logger.LogError("Failed to get user ID from Twitter API. Response was empty");

                response.AddError("Failed to get user ID from Twitter API", "");

                return response;
            }

            var getUserIdResponseClass = JsonSerializer.Deserialize<GetAccountIdApiResponse>(getUserIdResponseString);

            if (getUserIdResponseClass == null || getUserIdResponseClass.Data == null || string.IsNullOrEmpty(getUserIdResponseClass.Data.Id))
            {
                _logger.LogError("Failed to deserialize user ID from Twitter API response");

                response.AddError("Failed to deserialize user ID from Twitter API response", "");

                return response;
            }

            string userId = getUserIdResponseClass.GetId();

            await _settingStore.AddOrUpdateSetting(Guid.Parse(SettingKeys.XUserId), applicationTenantId, "X/Twitter Identifier", userId);

            var mediaFolderRepo = _dataStore.GetRepository<MediaFolder>();

            var tweetsFolder = new MediaFolder(applicationTenantId, "Tweets", "");

            mediaFolderRepo.Create(tweetsFolder);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return response;
        }
    }

}
