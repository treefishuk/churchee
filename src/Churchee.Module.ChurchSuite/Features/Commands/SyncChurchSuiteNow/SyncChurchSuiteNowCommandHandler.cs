using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Module.ChurchSuite.Jobs;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Churchee.Module.ChurchSuite.Features.Commands.SyncChurchSuiteNow
{
    public class SyncChurchSuiteNowCommandHandler : IRequestHandler<SyncChurchSuiteNowCommand, CommandResponse>
    {
        private readonly IJobService _jobShedularService;
        private readonly ICurrentUser _currentUser;
        private readonly IDistributedCache _distributedCache;

        public SyncChurchSuiteNowCommandHandler(IJobService jobShedularService, ICurrentUser currentUser, IDistributedCache distributedCache)
        {
            _jobShedularService = jobShedularService;
            _currentUser = currentUser;
            _distributedCache = distributedCache;
        }

        public async Task<CommandResponse> Handle(SyncChurchSuiteNowCommand request, CancellationToken cancellationToken)
        {
            var response = new CommandResponse();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string cacheKey = $"ChurchSuiteQueued_{applicationTenantId}";

            string existing = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(existing))
            {
                response.AddError("A sync is already queued or running. Please wait 10 mins and try again", string.Empty);

                return response;
            }

            // Set the flag with a short expiration (e.g., 10 minutes)
            await _distributedCache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            }, cancellationToken);


            _jobShedularService.QueueJob<SyncChurchSuiteEventsJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));

            return response;
        }
    }
}
