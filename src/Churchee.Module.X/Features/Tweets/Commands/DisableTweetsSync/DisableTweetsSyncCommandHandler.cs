using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.x.Helpers;
using MediatR;

namespace Churchee.Module.X.Features.Tweets.Commands
{
    public class DisableTweetsSyncCommandHandler : IRequestHandler<DisableTweetsSyncCommand, CommandResponse>
    {
        private readonly IJobService _jobService;
        private readonly IDataStore _dataStore;
        private readonly ISettingStore _settingStore;

        public DisableTweetsSyncCommandHandler(IJobService jobService, IDataStore dataStore, ISettingStore settingStore)
        {
            _jobService = jobService;
            _dataStore = dataStore;
            _settingStore = settingStore;
        }

        public async Task<CommandResponse> Handle(DisableTweetsSyncCommand request, CancellationToken cancellationToken)
        {
            _jobService.RemoveScheduledJob($"{request.ApplicationTenantId}_SyncTweets");

            await _settingStore.ClearSetting(Guid.Parse(SettingKeys.XUserAccount), request.ApplicationTenantId);
            await _settingStore.ClearSetting(Guid.Parse(SettingKeys.XUserId), request.ApplicationTenantId);

            var tokenRepo = _dataStore.GetRepository<Token>();

            var token = tokenRepo.GetQueryable().FirstOrDefault(w => w.ApplicationTenantId == request.ApplicationTenantId && w.Key == SettingKeys.XBearerToken);

            if (token != null)
            {
                tokenRepo.PermanentDelete(token);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();

        }
    }

}
