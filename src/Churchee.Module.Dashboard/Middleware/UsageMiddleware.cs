using Churchee.Common.Abstractions.Auth;
using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Site.Entities;
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
                // Fire-and-forget background task, do not await or capture context
                _ = Task.Run(() => LogRequest(context, tenantResolver));

                await _next(context);
            }
        }

        internal async Task LogRequest(HttpContext context, ITenantResolver tenantResolver)
        {
            try
            {
                string ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string userAgent = context.Request.Headers.UserAgent.ToString();
                string url = context.Request.Path.ToString();
                string referrerFull = context.Request.Headers.Referer.ToString();
                string referrerHost = string.IsNullOrEmpty(referrerFull) ? string.Empty : new Uri(referrerFull).Host;

                // Exit early for requests with any file extension (e.g., .css, .js, .png, .php, .html etc.)
                if (Path.HasExtension(url))
                {
                    return;
                }

                // Exit early for requests with no user agent, this is to avoid logging requests from bots or crawlers
                if (string.IsNullOrEmpty(userAgent))
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

                // If device is null or empty, we don't log the request
                if (string.IsNullOrEmpty(device))
                {
                    return;
                }

                if (!string.IsNullOrEmpty(ipAddress))
                {

                    var tenantId = tenantResolver.GetTenantId();

                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DbContext>();

                    bool pageExists = await dbContext.Set<WebContent>().AnyAsync(p => p.ApplicationTenantId == tenantId && p.Url == url);

                    if (!pageExists)
                    {
                        // If the page does not exist, we don't log the request
                        return;
                    }


                    var pageView = new PageView(tenantId)
                    {
                        IpAddress = ipAddress,
                        UserAgent = userAgent,
                        Url = url,
                        Referrer = referrerHost,
                        ReferrerFull = referrerFull,
                        Device = device,
                        OS = os,
                        Browser = browser,
                        ViewedAt = DateTime.UtcNow
                    };

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
