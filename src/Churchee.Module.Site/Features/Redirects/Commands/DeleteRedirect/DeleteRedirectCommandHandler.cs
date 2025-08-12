using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Redirects.Commands
{
    public class DeleteRedirectCommandHandler : IRequestHandler<DeleteRedirectCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public DeleteRedirectCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(DeleteRedirectCommand request, CancellationToken cancellationToken)
        {
            await _storage.GetRepository<RedirectUrl>().SoftDelete(request.RedirectId);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
