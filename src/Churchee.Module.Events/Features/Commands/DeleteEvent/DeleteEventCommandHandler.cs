using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using MediatR;

namespace Churchee.Module.Events.Features.Commands
{
    public class DeleteEventCommandHandler : IRequestHandler<DeleteEventCommand, CommandResponse>
    {
        private readonly IDataStore _dataStore;

        public DeleteEventCommandHandler(IDataStore dataStore)
        {
            _dataStore = dataStore;
        }

        public async Task<CommandResponse> Handle(DeleteEventCommand request, CancellationToken cancellationToken)
        {
            await _dataStore.GetRepository<Event>().SoftDelete(request.EventId);

            await _dataStore.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
