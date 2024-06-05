using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Events;
using MediatR;

namespace Churchee.Module.Tenancy.Features.Churches.Commands
{
    public class AddChurchCommandHandler : IRequestHandler<AddChurchCommand, CommandResponse>
    {

        private readonly IDataStore _store;

        public AddChurchCommandHandler(IDataStore store)
        {
            _store = store;
        }

        public async Task<CommandResponse> Handle(AddChurchCommand request, CancellationToken cancellationToken)
        {
            var newEntity = new ApplicationTenant(Guid.NewGuid(), request.Name, request.CharityNumber);

            newEntity.AddDomainEvent(new TenantAddedEvent(newEntity.Id));

            _store.GetRepository<ApplicationTenant>().Create(newEntity);

            await _store.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
