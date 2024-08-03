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
                       "img-src data: https:; " +
                       "object-src 'none'; " +
                       "script-src 'self'; " +
                       "style-src 'self' fonts.googleapis.com;" +
                       "font-src 'self' fonts.gstatic.com; " +
                       "frame-ancestors 'none'; " +
                       "upgrade-insecure-requests; ";

            context.Response.Headers.Append("Content-Security-Policy", csp);

        }

        return _next(context);
    }
}


