using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Helpers;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands.CreatePageType
{
    public class CreatePageTypeCommandHandler : IRequestHandler<CreatePageTypeCommand, CommandResponse>
    {
        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public CreatePageTypeCommandHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }

        public async Task<CommandResponse> Handle(CreatePageTypeCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var repo = _storage.GetRepository<PageType>();

            var newPageType = new PageType(Guid.NewGuid(), applicationTenantId, true, request.Name);

            repo.Create(newPageType);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
