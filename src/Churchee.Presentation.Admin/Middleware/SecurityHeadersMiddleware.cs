public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;

    public SecurityHeadersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public Task Invoke(HttpContext context)
    {
        return BeginInvoke(context);
    }

    private Task BeginInvoke(HttpContext context)
    {
        bool isAdmin = context.Request.Path.StartsWithSegments("/management");

        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("Permissions-Policy", "accelerometer=(), autoplay=(), camera=(), cross-origin-isolated=(), display-capture=(), encrypted-media=(), fullscreen=(), geolocation=(), gyroscope=(), keyboard-map=(), magnetometer=(), microphone=(), midi=(), payment=(), picture-in-picture=(), publickey-credentials-get=(), screen-wake-lock=(), sync-xhr=(self), usb=(), web-share=(), xr-spatial-tracking=(), clipboard-read=(), clipboard-write=(), gamepad=(), hid=(), idle-detection=(), interest-cohort=(), serial=(), unload=()");

        if (isAdmin)
        {
            string csp = "base-uri 'self'; " +
                       "default-src 'self'; " +
                       "img-src data: https:; " +
                       "object-src 'none'; " +
                       "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                       "style-src 'self' 'unsafe-inline' fonts.googleapis.com;" +
                       "font-src 'self' fonts.gstatic.com; " +
                       "frame-ancestors 'none'; " +
                       "upgrade-insecure-requests; ";

            context.Response.Headers.Append("Content-Security-Policy", csp);

        }
        else
        {
            string csp = "base-uri 'self'; " +
                       "default-src 'self'; " +
                       "img-src 'self'; " +
                       "object-src 'none'; " +
                       "script-src 'self'; " +
                       "style-src 'self';" +
                       "font-src 'self'; " +
                       "frame-ancestors 'none'; " +
                       "upgrade-insecure-requests; ";

            context.Response.Headers.Append("Content-Security-Policy", csp);

        }

        return _next(context);
    }
}


