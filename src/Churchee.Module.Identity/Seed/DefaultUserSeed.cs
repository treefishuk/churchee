using Churchee.Module.Identity.Abstractions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Churchee.Module.Identity
{
    public class DefaultUserSeed : IIdentitySeed
    {
        /// <summary>
        /// The private user manager.
        /// </summary>
        private readonly ChurcheeUserManager _userManager;
        private readonly DbContext _context;

        public DefaultUserSeed(ChurcheeUserManager userManager, DbContext context)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task CreateAsync()
        {
            if (!_context.Set<ApplicationUser>().IgnoreQueryFilters().Any())
            {
                var user = new ApplicationUser(
                    applicationTenantId: Guid.Parse("2ca25984-b0f6-44e9-98ff-151d7d79dcbd"),
                    userName: "admin@churchee.com",
                    email: "admin@churchee.com");

                var createResult = await _userManager.CreateAsync(user, "C4urc433Pa55w0rd!");

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                await _userManager.ConfirmEmailAsync(user, token);

                await _userManager.AddClaimAsync(user, new Claim("ActiveTenantId", Guid.Empty.ToString()));
                await _userManager.AddClaimAsync(user, new Claim("ActiveTenantName", "Default"));

                if (createResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "SysAdmin");
                    await _userManager.AddToRoleAsync(user, "Admin");
                    await _userManager.AddToRoleAsync(user, "Developer");
                }
            }
        }
    }
}
