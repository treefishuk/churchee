using Churchee.Common.ValueTypes;
using MediatR;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesQuery : IRequest<IEnumerable<MultiSelectItem>>
    {
    }
}
