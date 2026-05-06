using Churchee.Common.Abstractions.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Threading.Tasks;

namespace Churchee.Module.Logging.Middleware
{
    public class UserLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public UserLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var currentUser = context.RequestServices.GetService<ICurrentUser>();

                if (currentUser == null)
                {
                    await _next(context);

                    return;
                }

                string userId = currentUser.GetUserId() ?? "anonymous";
                string tenantName = await currentUser.GetApplicationTenantName() ?? "Unknown";
                string userName = context.User.Identity?.Name ?? "anonymous";

                ILogEventEnricher[] enrichers =
                {
                    new PropertyEnricher("UserId", userId),
                    new PropertyEnricher("Tenant", tenantName),
                    new PropertyEnricher("Username", userName)
                };

                using (LogContext.Push(enrichers))
                {
                    await _next(context);
                }
            }
            catch
            {
                // don't block requests if logging enrichment fails
                await _next(context);
            }
        }
    }
}