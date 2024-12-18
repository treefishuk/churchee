using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Churchee.Module.Identity.Stores
{
    public class ChurcheeUserStore : UserStore<ApplicationUser, ApplicationRole, DbContext, Guid>
    {
        public ChurcheeUserStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }
    }
}
