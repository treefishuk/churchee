using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using MediatR;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries
{
    internal class GetPodcastSettingsRequestHandler : IRequestHandler<GetPodcastSettingsRequest, GetPodcastSettingsResponse>
    {
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _store;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;

        public GetPodcastSettingsRequestHandler(ISettingStore store, ICurrentUser currentUser, IJobService jobService)
        {
            _store = store;
            _currentUser = currentUser;
            _jobService = jobService;
        }

        public async Task<GetPodcastSettingsResponse> Handle(GetPodcastSettingsRequest request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string spotifyUrl = await _store.GetSettingValue(Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a"), applicationTenantId);

            string pageNameForPodcasts = await _store.GetSettingValue(_podcastsNameId, applicationTenantId);

            var lastRun = _jobService.GetLastRunDate($"{applicationTenantId}_SpotifyPodcasts");

            return new GetPodcastSettingsResponse(spotifyUrl, pageNameForPodcasts, lastRun);
        }
    }
}
