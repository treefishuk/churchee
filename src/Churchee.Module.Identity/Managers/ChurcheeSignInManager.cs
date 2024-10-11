using Churchee.Module.Identity.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Managers
{
    public class ChurcheeSignInManager : SignInManager<ApplicationUser>, ISignInManager
    {
        private readonly IIdentitySeed _identitySeed;

        public ChurcheeSignInManager(ChurcheeUserManager userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<ChurcheeSignInManager> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation,
            IIdentitySeed identitySeed) : base(
                userManager ?? throw new ArgumentNullException(nameof(userManager)),
                contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor)),
                claimsFactory ?? throw new ArgumentNullException(nameof(claimsFactory)),
                optionsAccessor ?? throw new ArgumentNullException(nameof(optionsAccessor)),
                logger ?? throw new ArgumentNullException(nameof(logger)),
                schemes ?? throw new ArgumentNullException(nameof(schemes)),
                confirmation ?? throw new ArgumentNullException(nameof(confirmation)))
        {
            _identitySeed = identitySeed ?? throw new ArgumentNullException(nameof(identitySeed));
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            await _identitySeed.CreateAsync();

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }
}
