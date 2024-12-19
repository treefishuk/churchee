using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Storage;
using Hangfire;
using Hangfire.Storage;
using MediatR;

namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries
{
    internal class GetPodcastSettingsRequestHandler : IRequestHandler<GetPodcastSettingsRequest, GetPodcastSettingsResponse>
    {
        private readonly Guid _podcastsNameId = Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d");
        private readonly ISettingStore _store;
        private readonly ICurrentUser _currentUser;

        public GetPodcastSettingsRequestHandler(ISettingStore store, ICurrentUser currentUser)
        {
            _store = store;
            _currentUser = currentUser;
        }

        public async Task<GetPodcastSettingsResponse> Handle(GetPodcastSettingsRequest request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string spotifyUrl = await _store.GetSettingValue(Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a"), applicationTenantId);

            string pageNameForPodcasts = await _store.GetSettingValue(_podcastsNameId, applicationTenantId);

            DateTime? lastRun = null;

            using (var connection = JobStorage.Current.GetConnection())
            {
                var recurringJobs = connection.GetRecurringJobs();

                lastRun = recurringJobs.Where(w => w.Id == $"{applicationTenantId}_SpotifyPodcasts").Select(s => s.LastExecution).FirstOrDefault();
            }

            return new GetPodcastSettingsResponse(spotifyUrl, pageNameForPodcasts, lastRun);
        }
    }
}
