using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Churchee.Module.Dashboard.Middleware
{
    public class UsageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UsageMiddleware> _logger;
        private readonly IServiceProvider _serviceProvider;
        public UsageMiddleware(RequestDelegate next, ILogger<UsageMiddleware> logger, IServiceProvider serviceProvider)
        {
            _next = next;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public async Task InvokeAsync(HttpContext context, ITenantResolver tenantResolver)
        {

            if (tenantResolver.GetTenantId() != Guid.Empty)
            {
                _ = Task.Run(async () => { await LogRequest(context, tenantResolver); });
            }

            await _next(context);
        }

        internal async Task LogRequest(HttpContext context, ITenantResolver tenantResolver)
        {
            try
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string userAgent = context.Request.Headers.UserAgent.ToString();
                string url = context.Request.Path.ToString();
                string referrer = context.Request.Headers.Referer.ToString();

                // Exit early for requests with any file extension (e.g., .css, .js, .png, .php, .html etc.)
                if (Path.HasExtension(url))
                {
                    return;
                }

                var deviceDetector = new DeviceDetectorNET.DeviceDetector(userAgent);

                deviceDetector.Parse();

                if (deviceDetector.IsBot())
                {
                    return;
                }

                string device = deviceDetector.GetDeviceName();         // Mobile, Tablet, Desktop
                string os = deviceDetector.GetOs().Match?.Name;          // Windows, Android, iOS
                string browser = deviceDetector.GetClient().Match?.Name; // Chrome, Firefox, etc.

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    var pageView = new PageView(tenantResolver.GetTenantId())
                    {
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Url = url,
                        Referrer = referrer,
                        Device = device,
                        OS = os,
                        Browser = browser,
                        ViewedAt = DateTime.UtcNow
                    };

                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                    dbContext.Set<PageView>().Add(pageView);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Usage Middleware Error");
            }
        }
    }
}
