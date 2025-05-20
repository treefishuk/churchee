using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.PageTypes.Commands
{
    public class DeletePageTypeCommandHandler : IRequestHandler<DeletePageTypeCommand, CommandResponse>
    {
        private readonly IDataStore _storage;

        public DeletePageTypeCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(DeletePageTypeCommand request, CancellationToken cancellationToken)
        {
            await _storage.GetRepository<PageType>().SoftDelete(request.Id);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
