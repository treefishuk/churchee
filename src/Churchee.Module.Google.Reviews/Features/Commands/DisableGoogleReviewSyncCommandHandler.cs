using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Google.Reviews.Helpers;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class DisableGoogleReviewSyncCommandHandler : IRequestHandler<DisableGoogleReviewSyncCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public DisableGoogleReviewSyncCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(DisableGoogleReviewSyncCommand request, CancellationToken cancellationToken)
        {
            var repo = _dataStore.GetRepository<Token>();

            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var accessToken = await repo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.GoogleReviewsAccessTokenKey.ToString(), applicationTenantId), cancellationToken);

            repo.PermanentDelete(accessToken);

            var refreshToken = await repo.FirstOrDefaultAsync(new GetTokenByKeySpecification(SettingKeys.GoogleReviewsRefreshTokenKey.ToString(), applicationTenantId), cancellationToken);

            repo.PermanentDelete(refreshToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
