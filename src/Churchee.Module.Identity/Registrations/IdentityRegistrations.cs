using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Identity.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Module.Identity.Registration
{
    public class IdentityRegistrations : IConfigureServicesAction
    {
        public int Priority => 1;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {
            serviceCollection.AddScoped<UserManager<ApplicationUser>, ChurcheeUserManager>();
            serviceCollection.AddScoped<ChurcheeUserManager, ChurcheeUserManager>();
            serviceCollection.AddScoped<IIdentitySeed, DefaultUserSeed>();
            serviceCollection.AddScoped<ISignInManager, ChurcheeSignInManager>();

        }
    }
}
