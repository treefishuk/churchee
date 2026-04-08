using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Events.Helpers;
using MediatR;

namespace Churchee.Module.ChurchSuite.Features.Queries.ChurchSuiteConfigured
{
    public class ChurchSuiteConfiguredQueryHandler : IRequestHandler<ChurchSuiteConfiguredQuery, bool>
    {
        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;

        public ChurchSuiteConfiguredQueryHandler(ISettingStore settingStore, ICurrentUser currentUser)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(ChurchSuiteConfiguredQuery request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string setting = await _settingStore.GetSettingValue(Guid.Parse(SettingKeys.ChurchSuiteEventsUrl), applicationTenantId);

            if (string.IsNullOrEmpty(setting))
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);

        }
    }
}
