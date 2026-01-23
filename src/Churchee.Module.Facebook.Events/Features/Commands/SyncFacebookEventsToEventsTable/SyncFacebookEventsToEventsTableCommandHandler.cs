using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Module.Facebook.Events.Jobs;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Facebook.Events.Features.Commands.SyncFacebookEventsToEventsTable
{
    public class SyncFacebookEventsToEventsTableCommandHandler : IRequestHandler<SyncFacebookEventsToEventsTableCommand, CommandResponse>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobShedularService;
        private readonly ILogger<SyncFacebookEventsToEventsTableCommandHandler> _logger;

        public SyncFacebookEventsToEventsTableCommandHandler(ICurrentUser currentUser, ILogger<SyncFacebookEventsToEventsTableCommandHandler> logger, IJobService jobShedularService)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jobShedularService = jobShedularService;
        }

        public async Task<CommandResponse> Handle(SyncFacebookEventsToEventsTableCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            try
            {
                var applicationTenantId = await _currentUser.GetApplicationTenantId();

                _jobShedularService.ScheduleJob<SyncFacebookEventsJob>($"{applicationTenantId}_FacebookEvents", a => a.ExecuteAsync(applicationTenantId, CancellationToken.None), Cron.Hourly);

                _jobShedularService.QueueJob<SyncFacebookEventsJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing Facebook events");

                response.AddError("Failed To Sync", "");
            }

            return response;
        }
    }
}
