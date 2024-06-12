﻿using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Hangfire;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Podcasts.Anchor.Features.Podcasts.Commands
{
    public class DisableAnchorPodcastSyncCommandHandler : IRequestHandler<DisableAnchorPodcastSyncCommand, CommandResponse>
    {
        private readonly ISettingStore _settingStore;
        private readonly Guid _rssFeedSettingKey = Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a");
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<DisableAnchorPodcastSyncCommandHandler> _logger;
        private readonly IDataStore _dataStore;


        public DisableAnchorPodcastSyncCommandHandler(ISettingStore settingStore, ICurrentUser currentUser, ILogger<DisableAnchorPodcastSyncCommandHandler> logger, IRecurringJobManager recurringJobManager, IDataStore dataStore)
        {
            _settingStore = settingStore;
            _currentUser = currentUser;
            _logger = logger;
            _recurringJobManager = recurringJobManager;
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(DisableAnchorPodcastSyncCommand request, CancellationToken cancellationToken)
        {
            await _settingStore.ClearSetting(_rssFeedSettingKey, Guid.Parse("2ca25984-b0f6-44e9-98ff-151d7d79dcbd"));

            _recurringJobManager.RemoveIfExists($"{await _currentUser.GetApplicationTenantId()}_AnchorPodcasts");

            var anchorPodcasts = _dataStore.GetRepository<Podcast>().GetQueryable().Where(w => w.SourceName == "Spotify").ToList();

            foreach (var item in anchorPodcasts)
            {
                _dataStore.GetRepository<Podcast>().PermenantDelete(item);
            }

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
