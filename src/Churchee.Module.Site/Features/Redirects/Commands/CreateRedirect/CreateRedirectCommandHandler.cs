using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Redirects.Commands
{
    public class CreateRedirectCommandHandler : IRequestHandler<CreateRedirectCommand, CommandResponse>
    {
        private readonly IDataStore _storage;
        private static readonly char PathSeparator = '/';

        public CreateRedirectCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(CreateRedirectCommand request, CancellationToken cancellationToken)
        {
            if (!request.Path.StartsWith(PathSeparator))
            {
                request.Path = PathSeparator + request.Path;
            }

            _storage.GetRepository<RedirectUrl>().Create(new RedirectUrl(path: request.Path, request.WebContentId));

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
