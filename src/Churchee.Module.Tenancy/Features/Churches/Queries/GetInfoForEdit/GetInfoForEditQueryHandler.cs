using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Specifications;
using MediatR;

namespace Churchee.Module.Tenancy.Features.Churches.Queries
{
    public class GetInfoForEditQueryHandler : IRequestHandler<GetInfoForEditQuery, GetInfoForEditResponse>
    {

        private readonly IDataStore _store;

        public GetInfoForEditQueryHandler(IDataStore store)
        {
            _store = store;
        }

        public async Task<GetInfoForEditResponse> Handle(GetInfoForEditQuery request, CancellationToken cancellationToken)
        {
            var response = await _store.GetRepository<ApplicationTenant>()
                .FirstOrDefaultAsync(new ApplicationTenantByIdIncludingHostsSpecification(request.Id), s => new GetInfoForEditResponse
                {
                    CharityNumber = s.CharityNumber,
                    Domains = s.Hosts.Select(h => h.Host).ToList()
                }, cancellationToken);

            response.Domains.Add(string.Empty);
            response.Domains.Add(string.Empty);
            response.Domains.Add(string.Empty);

            return response;
        }
    }

}
