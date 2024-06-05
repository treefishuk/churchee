using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Identity.Stores
{
    public class ChurcheeUserStore : UserStore<ApplicationUser, ApplicationRole, DbContext, Guid>
    {
        public ChurcheeUserStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
        }

        public override Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            ThrowIfDisposed();

            return Users.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName, cancellationToken);
        }

    }
}
