using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Dashboard.Middleware
{
    public class UsageMiddleware
    {
        private readonly RequestDelegate _next;

        public UsageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DbContext dbContext, ITenantResolver tenantResolver)
        {
            _ = Task.Run(async () => { await LogRequest(context, dbContext, tenantResolver); });

            await _next(context);
        }

        private async Task LogRequest(HttpContext context, DbContext dbContext, ITenantResolver tenantResolver)
        {
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var url = context.Request.Path.ToString();
            var referrer = context.Request.Headers["Referer"].ToString();

            if (url.EndsWith(".css"))
            {
                return;
            }

            var deviceDetector = new DeviceDetectorNET.DeviceDetector(userAgent);

            if (deviceDetector.IsBot())
            {
                return;
            }

            deviceDetector.Parse();

            var device = deviceDetector.GetDeviceName();         // Mobile, Tablet, Desktop
            var os = deviceDetector.GetOs().Match.Name;          // Windows, Android, iOS
            var browser = deviceDetector.GetClient().Match.Name; // Chrome, Firefox, etc.

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

                dbContext.Set<PageView>().Add(pageView);

                await dbContext.SaveChangesAsync();
            }
        }
    }
}
