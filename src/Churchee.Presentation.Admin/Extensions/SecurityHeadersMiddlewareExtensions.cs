public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeadersMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
