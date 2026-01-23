using Churchee.Common.Abstractions.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Churchee.Module.Tenancy.Infrastructure
{
    public class ClaimTenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public ClaimTenantResolver(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public Guid GetTenantId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Guid.Empty;
            }

            var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == "ActiveTenantId");

            if (claim == null)
            {
                return Guid.Empty;
            }

            return Guid.Parse(claim.Value);
        }

        public string GetTenantDevName()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return string.Empty;
            }

            var claim = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(w => w.Type == "ActiveTenantName");

            if (claim == null)
            {
                return string.Empty;
            }

            return claim.Value.ToLowerInvariant();
        }

        public string GetCDNPrefix()
        {
            string urlPrefix = _configuration.GetRequiredSection("Images")["Prefix"] ?? string.Empty;

            return urlPrefix.Replace("*", GetTenantDevName());
        }
    }
}
