using MediatR;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Features.Contributors.Queries
{
    public class ListContributorsForTenantQuery : IRequest<IEnumerable<ListContributorsForTenantResponseItem>>
    {
    }
}
