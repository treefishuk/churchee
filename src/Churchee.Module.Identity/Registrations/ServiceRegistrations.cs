using Churchee.Common.Abstractions.Extensibility;
using Churchee.Module.Identity.Abstractions;
using Churchee.Module.Identity.Requirements;
using Churchee.Module.Identity.Seed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Churchee.Module.Identity.Registrations
{
    public class ServiceRegistrations : IConfigureAdminServicesAction
    {
        public int Priority => 1;

        public void Execute(IServiceCollection serviceCollection, IServiceProvider serviceProvider)
        {

            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            serviceCollection.AddScoped<UserManager<ApplicationUser>, ChurcheeUserManager>();
            serviceCollection.AddScoped<ChurcheeUserManager, ChurcheeUserManager>();
            serviceCollection.AddScoped<IIdentitySeed, DefaultUserSeed>();
            serviceCollection.AddScoped<ISignInManager, ChurcheeSignInManager>();

            serviceCollection.AddAuthorizationBuilder()
                .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .AddRequirements(new ChurcheeAuthorizationRequirement())
                    .Build());

            serviceCollection.Configure<IdentityOptions>(configuration.GetSection("IdentityOptions"));
        }
    }
}
