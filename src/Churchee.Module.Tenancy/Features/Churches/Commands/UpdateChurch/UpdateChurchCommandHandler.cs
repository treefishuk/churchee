using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Specifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Features.Churches.Commands.UpdateChurch
{
    public class UpdateChurchCommandHandler : IRequestHandler<UpdateChurchCommand, CommandResponse>
    {
        private readonly IDataStore _store;

        public UpdateChurchCommandHandler(IDataStore store)
        {
            _store = store;
        }

        public async Task<CommandResponse> Handle(UpdateChurchCommand request, CancellationToken cancellationToken)
        {
            var repo = _store.GetRepository<ApplicationTenant>();

            var entity = await repo.GetByIdAsync(request.Id, cancellationToken);

            entity.SetCharityNumber(request.CharityNumber);

            await UpdateHosts(request.Id, request.Domains, cancellationToken);

            await _store.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private async Task UpdateHosts(Guid applicationTenantId, List<string> domains, CancellationToken cancellationToken)
        {
            var repo = _store.GetRepository<ApplicationHost>();

            var existingHosts = await repo.GetListAsync(new ApplicationHostsByApplicationTenantIdSpecification(applicationTenantId), cancellationToken);

            // Remove entries from existingHosts whose domain appears in domains
            var domainsSet = domains.Where(d => !string.IsNullOrEmpty(d)).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var hostsToRemove = existingHosts.Where(h => !domainsSet.Contains(h.Host)).ToList();

            foreach (var host in hostsToRemove)
            {
                repo.PermanentDelete(host);
            }

            // Add new entries for domains not in existingHosts
            var existingDomains = existingHosts.Select(h => h.Host).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var newDomains = domains.Where(d => !string.IsNullOrEmpty(d) && !existingDomains.Contains(d)).ToList();

            foreach (string domain in newDomains)
            {
                var newHost = new ApplicationHost(domain, applicationTenantId);

                repo.Create(newHost);
            }


        }
    }
}
