using Churchee.Common.ValueTypes;
using Churchee.CQRS.Abstractions;
using System;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesForUserQuery : IRequest<IEnumerable<MultiSelectItem>>
    {
        public GetAllSelectableRolesForUserQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; set; }
    }
}
