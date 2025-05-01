using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.x.Helpers;
using MediatR;

namespace Churchee.Module.X.Features.Tweets.Queries.CheckXIntergrationStatus
{
    public class CheckXIntegrationStatusQueryHandler : IRequestHandler<CheckXIntegrationStatusQuery, CheckXIntegrationStatusResponse>
    {

        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;

        public CheckXIntegrationStatusQueryHandler(ISettingStore settingStore, ICurrentUser currentUser, IJobService jobService)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _jobService = jobService;
        }

        public async Task<CheckXIntegrationStatusResponse> Handle(CheckXIntegrationStatusQuery request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string userId = await _settingStore.GetSettingValue(Guid.Parse(SettingKeys.XUserId), applicationTenantId);

            if (string.IsNullOrEmpty(userId))
            {
                return new CheckXIntegrationStatusResponse
                {
                    Configured = false
                };
            }

            var lastRun = _jobService.GetLastRunDate($"{applicationTenantId}_SyncTweets");


            return new CheckXIntegrationStatusResponse
            {
                Configured = true,
                LastRun = lastRun
            };
        }
    }
}
