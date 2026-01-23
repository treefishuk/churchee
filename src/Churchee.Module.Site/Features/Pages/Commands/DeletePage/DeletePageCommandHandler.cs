using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using MediatR;

namespace Churchee.Module.Site.Features.Pages.Commands.UpdatePage
{
    public class DeletePageCommandHandler : IRequestHandler<DeletePageCommand, CommandResponse>
    {

        private readonly IDataStore _storage;

        public DeletePageCommandHandler(IDataStore storage)
        {
            _storage = storage;
        }

        public async Task<CommandResponse> Handle(DeletePageCommand request, CancellationToken cancellationToken)
        {
            await _storage.GetRepository<Page>().SoftDelete(request.PageId);

            await _storage.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
