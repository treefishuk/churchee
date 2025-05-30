using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
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

            var entity = await repo.GetByIdAsync(request.Id);

            entity.SetCharityNumber(request.CharityNumber);

            UpdateHosts(request.Id, request.Domains);

            await _store.SaveChangesAsync(cancellationToken);

            return new CommandResponse();
        }

        private void UpdateHosts(Guid applicationTenantId, List<string> domains)
        {
            var repo = _store.GetRepository<ApplicationHost>();

            var existingHosts = repo
                .GetQueryable()
                .IgnoreQueryFilters()
                .Where(x => x.ApplicationTenantId == applicationTenantId)
                .ToList();

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
