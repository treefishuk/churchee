using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.Events.Helpers;
using Churchee.Module.ChurchSuite.Jobs;
using Hangfire;
using MediatR;

namespace Churchee.Module.ChurchSuite.Features.Commands.EnableChurchSuiteIntegration
{
    public class EnableChurchSuiteIntegrationCommandHandler : IRequestHandler<EnableChurchSuiteIntegrationCommand, CommandResponse>
    {

        private readonly ISettingStore _settingStore;
        private readonly ICurrentUser _currentUser;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IJobService _jobService;

        public EnableChurchSuiteIntegrationCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, IHttpClientFactory clientFactory, IJobService jobService)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _clientFactory = clientFactory;
            _jobService = jobService;
        }

        public async Task<CommandResponse> Handle(EnableChurchSuiteIntegrationCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            await _settingStore.AddOrUpdateSetting(Guid.Parse(SettingKeys.ChurchSuiteEventsUrl), applicationTenantId, "Church Suite Integration Url", request.Url);

            var httpResponse = await _clientFactory.CreateClient().GetAsync(request.Url, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                response.AddError("Unable to connect to the feed. Please ensure the sub domain is correct and try again.", nameof(request.Url));

                return response;
            }

            _jobService.ScheduleJob<SyncChurchSuiteEventsJob>($"{applicationTenantId}_SyncChurchSuiteEvents", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Hourly);

            return response;
        }
    }
}
