using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using MediatR;

namespace Churchee.Module.Jotform.Features.Commands
{
    public class ConfigureJotformCommandHandler : IRequestHandler<ConfigureJotformCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;
        private readonly ICurrentUser _currentUser;

        public ConfigureJotformCommandHandler(IDataStore dataStore, ICurrentUser currentUser)
        {
            _dataStore = dataStore;
            _currentUser = currentUser;
        }
        public async Task<CommandResponse> Handle(ConfigureJotformCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var repo = _dataStore.GetRepository<Token>();

            var newToken = new Token(applicationTenantId, "Jotform", request.ApiKey);

            repo.Create(newToken);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
