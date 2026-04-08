using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Events.Helpers;
using MediatR;

namespace Churchee.Module.ChurchSuite.Features.Commands.DisableChurchSuiteSync
{
    public class DisableChurchSuiteSyncCommandHandler : IRequestHandler<DisableChurchSuiteSyncCommand, CommandResponse>
    {

        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;

        public DisableChurchSuiteSyncCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IJobService jobService)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _jobService = jobService;
        }

        public async Task<CommandResponse> Handle(DisableChurchSuiteSyncCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.ClearSetting(Guid.Parse(SettingKeys.ChurchSuiteEventsUrl), applicationTenantId);

            _jobService.RemoveScheduledJob($"{applicationTenantId}_SyncChurchSuiteEvents");

            return new CommandResponse();
        }
    }
}
