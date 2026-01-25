using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.YouTube.Helpers;
using MediatR;

namespace Churchee.Module.YouTube.Features.YouTube.Queries
{
    internal class GetYouTubeSettingsRequestHandler : IRequestHandler<GetYouTubeSettingsRequest, GetYouTubeSettingsResponse>
    {
        private readonly ISettingStore _store;
        private readonly ICurrentUser _currentUser;
        private readonly IJobService _jobService;

        public GetYouTubeSettingsRequestHandler(ISettingStore store, ICurrentUser currentUser, IJobService jobService)
        {
            _store = store;
            _currentUser = currentUser;
            _jobService = jobService;
        }

        public async Task<GetYouTubeSettingsResponse> Handle(GetYouTubeSettingsRequest request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            string pageNameForVideos = await _store.GetSettingValue(SettingKeys.VideosPageName, applicationTenantId);

            string handle = await _store.GetSettingValue(SettingKeys.Handle, applicationTenantId);

            var lastRun = _jobService.GetLastRunDate($"{applicationTenantId}_YouTubeVideos");

            return new GetYouTubeSettingsResponse(handle, pageNameForVideos, lastRun);
        }
    }
}
