using Churchee.Common.ValueTypes;
using Churchee.CQRS.Abstractions;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesQuery : IRequest<IEnumerable<MultiSelectItem>>
    {
    }
}
