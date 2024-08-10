using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Churchee.Module.Hangfire.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {

            var httpContext = context.GetHttpContext();

            bool authenticated = httpContext.User.Identity.IsAuthenticated;

            bool isSysAdmin = httpContext.User.IsInRole("SysAdmin");

            if (authenticated && isSysAdmin)
            {
                return true;
            };

            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            return false;
        }
    }
}
