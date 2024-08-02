using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Requirements
{
    public class ChurcheeAuthorizationRequirement : AuthorizationHandler<ChurcheeAuthorizationRequirement>, IAuthorizationRequirement
    {

        /// <summary>
        /// Makes a decision if authorization is allowed based on a specific requirement.
        /// </summary>
        /// <param name="context">The authorization context.</param>
        /// <param name="requirement">The requirement to evaluate.</param>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ChurcheeAuthorizationRequirement requirement)
        {
            bool isAdmin = context.Resource is HttpContext httpContext && httpContext.Request.Path.StartsWithSegments("/management");

            if (!isAdmin)
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            var user = context.User;
            var userIsAnonymous =
                user?.Identity == null ||
                !user.Identities.Any(i => i.IsAuthenticated);
            if (!userIsAnonymous)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(DenyAnonymousAuthorizationRequirement)}: Requires an authenticated user.";
        }
    }
}
