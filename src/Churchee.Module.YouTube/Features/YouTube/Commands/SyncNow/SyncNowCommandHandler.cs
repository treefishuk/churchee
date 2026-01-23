using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Module.YouTube.Jobs;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.SyncNow
{
    public class SyncNowCommandHandler : IRequestHandler<SyncNowCommand, CommandResponse>
    {
        private readonly IJobService _jobService;
        private readonly ICurrentUser _currentUser;
        private readonly IDistributedCache _distributedCache;

        public SyncNowCommandHandler(IJobService jobService, ICurrentUser currentUser, IDistributedCache distributedCache)
        {
            _jobService = jobService;
            _currentUser = currentUser;
            _distributedCache = distributedCache;
        }

        public async Task<CommandResponse> Handle(SyncNowCommand request, CancellationToken cancellationToken)
        {

            var response = new CommandResponse();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string cacheKey = $"YouTubeSyncJobQueued_{applicationTenantId}";

            string? existing = await _distributedCache.GetStringAsync(cacheKey, cancellationToken);

            if (!string.IsNullOrEmpty(existing))
            {
                response.AddError("A YouTube sync job is already queued or running. Please wait for it to complete before queuing another.", "");

                return response;
            }

            // Set the flag with a short expiration (e.g., 10 minutes)
            await _distributedCache.SetStringAsync(cacheKey, "1", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            }, cancellationToken);

            _jobService.QueueJob<SyncYouTubeVideosJob>(a => a.ExecuteAsync(applicationTenantId, CancellationToken.None));

            return new CommandResponse();
        }
    }
}
