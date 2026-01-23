using Churchee.Common.Abstractions.Auth;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Templates.Commands
{
    public class CreateTemplateCommandHandler : IRequestHandler<CreateTemplateCommand, CommandResponse>
    {

        private readonly IDataStore _storage;
        private readonly ICurrentUser _currentUser;

        public CreateTemplateCommandHandler(IDataStore storage, ICurrentUser currentUser)
        {
            _storage = storage;
            _currentUser = currentUser;
        }


        public async Task<CommandResponse> Handle(CreateTemplateCommand request, CancellationToken cancellationToken)
        {
            var applicationTenantId = await _currentUser.GetApplicationTenantId();

            var repo = _storage.GetRepository<ViewTemplate>();

            var newTemplate = new ViewTemplate(applicationTenantId, request.Path, request.Content);

            repo.Create(newTemplate);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
