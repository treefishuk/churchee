using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;

namespace Churchee.Module.Identity.Managers
{
    public class ChurcheeUserManager : UserManager<ApplicationUser>
    {
        public ChurcheeUserManager(IUserStore<ApplicationUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<ChurcheeUserManager> logger)
            : base(store ?? throw new ArgumentNullException(nameof(store)),
                  optionsAccessor ?? throw new ArgumentNullException(nameof(optionsAccessor)),
                  passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher)),
                  userValidators ?? throw new ArgumentNullException(nameof(userValidators)),
                  passwordValidators ?? throw new ArgumentNullException(nameof(passwordValidators)),
                  keyNormalizer ?? throw new ArgumentNullException(nameof(keyNormalizer)),
                  errors ?? throw new ArgumentNullException(nameof(errors)),
                  services ?? throw new ArgumentNullException(nameof(services)),
                  logger ?? throw new ArgumentNullException(nameof(logger)))
        {
        }
    }
}
