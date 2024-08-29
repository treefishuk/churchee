using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Helpers;
using Churchee.Module.Tokens.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Facebook.Events.Features.Commands
{
    public class DisableFacebookSyncCommandHandler : IRequestHandler<DisableFacebookSyncCommand, CommandResponse>
    {

        private readonly Guid _pageId = Guid.Parse("3de048ae-d711-4609-9b66-97564a9d0d68");
        private readonly Guid _facebookUserId = Guid.Parse("efa5252a-e825-4310-a498-2383875e9584");

        private readonly ISettingStore _settingStore;
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<DisableFacebookSyncCommandHandler> _logger;
        private readonly IDataStore _dataStore;

        public DisableFacebookSyncCommandHandler(ISettingStore settingStore, IRecurringJobManager recurringJobManager, ICurrentUser currentUser, ILogger<DisableFacebookSyncCommandHandler> logger, IDataStore dataStore)
        {
            _settingStore = settingStore;
            _recurringJobManager = recurringJobManager;
            _currentUser = currentUser;
            _logger = logger;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(DisableFacebookSyncCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                _recurringJobManager.RemoveIfExists($"{_currentUser.GetApplicationTenantId()}_FacebookEvents");

                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                await _settingStore.ClearSetting(_pageId, applicationTenantId);

                await _settingStore.ClearSetting(_facebookUserId, applicationTenantId);

                var tokenRepo = _dataStore.GetRepository<Token>();

                var pageAccessToken = tokenRepo.GetQueryable().Where(w => w.ApplicationTenantId == applicationTenantId && w.Key == SettingKeys.FacebookPageAccessToken).FirstOrDefault();

                if (pageAccessToken != null)
                {
                    tokenRepo.PermenantDelete(pageAccessToken);
                }

                var facebookAccessToken = tokenRepo.GetQueryable().Where(w => w.ApplicationTenantId == applicationTenantId && w.Key == SettingKeys.FacebookAccessToken).FirstOrDefault();

                if (facebookAccessToken != null)
                {
                    tokenRepo.PermenantDelete(facebookAccessToken);
                }

                await _dataStore.SaveChangesAsync(cancellationToken);

            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Failed to disable Facebook Sync");

                response.AddError("Failed to disable Facebook Sync", "");
            }

            return response;
        }
    }
}
