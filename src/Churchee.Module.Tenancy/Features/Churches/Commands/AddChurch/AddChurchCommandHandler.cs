using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Events;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace Churchee.Module.Tenancy.Features.Churches.Commands
{
    public class AddChurchCommandHandler : IRequestHandler<AddChurchCommand, CommandResponse>
    {

        private readonly IDataStore _store;
        private readonly IConfiguration _configuration;

        public AddChurchCommandHandler(IDataStore store, IConfiguration configuration)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<CommandResponse> Handle(AddChurchCommand request, CancellationToken cancellationToken)
        {
            var newEntity = new ApplicationTenant(Guid.NewGuid(), request.Name, request.CharityNumber);

            string subDomainTemplate = _configuration.GetRequiredSection("Tenants").GetValue<string>("Domain") ?? string.Empty;

            string hostEntry = subDomainTemplate.Replace("*", newEntity.DevName);

            newEntity.AddHost(hostEntry);

            newEntity.AddDomainEvent(new TenantAddedEvent(newEntity.Id));

            _store.GetRepository<ApplicationTenant>().Create(newEntity);

            await _store.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }
    }
}
