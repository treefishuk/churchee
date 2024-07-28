﻿using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Features.Roles.Queries
{
    public class GetAllSelectableRolesForUserQueryHandler : IRequestHandler<GetAllSelectableRolesForUserQuery, IEnumerable<MultiSelectItem>>
    {

        private readonly IDataStore _store;
        private readonly ChurcheeUserManager _churcheeUserManager;

        public GetAllSelectableRolesForUserQueryHandler(IDataStore store, ChurcheeUserManager churcheeUserManager)
        {
            _store = store;
            _churcheeUserManager = churcheeUserManager;
        }

        public async Task<IEnumerable<MultiSelectItem>> Handle(GetAllSelectableRolesForUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _churcheeUserManager.FindByIdAsync(request.UserId.ToString());

            var userRoles = await _churcheeUserManager.GetRolesAsync(user);

            var rolesQuery = _store.GetRepository<ApplicationRole>().GetQueryable();

            var data = await _store.GetRepository<ApplicationRole>()
                .GetQueryable()
                .Where(w => w.Selectable)
                .Select(s => new MultiSelectItem { Value = s.Id, Text = s.Name, Selected = userRoles.Any(a => a == s.Name) })
                .ToListAsync(cancellationToken);

            return data;
        }
    }
}
