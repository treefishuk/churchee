using Churchee.Common.Abstractions.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Churchee.Module.Identity.Infrastructure
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ChurcheeUserManager _userManager;

        public CurrentUser(IHttpContextAccessor httpContextAccessor, ChurcheeUserManager userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<Guid> GetApplicationTenantId()
        {
            var claims = await GetClaims();

            var claim = claims.FirstOrDefault(w => w.Type == "ActiveTenantId");

            return claim != null ? Guid.Parse(claim.Value) : Guid.Empty;
        }

        public async Task<string> GetApplicationTenantName()
        {
            var claims = await GetClaims();

            var claim = claims.FirstOrDefault(w => w.Type == "ActiveTenantName");

            return claim?.Value;
        }
        private async Task<IList<Claim>> GetClaims()
        {
            string id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(id);

            var claims = await _userManager.GetClaimsAsync(user);

            return claims;
        }

        public bool HasFeature(string featureName)
        {
            return _httpContextAccessor.HttpContext.User.Claims.Any(w => w.Type == featureName);
        }

        public bool HasRole(string roleName)
        {
            return _httpContextAccessor.HttpContext.User.IsInRole(roleName) || _httpContextAccessor.HttpContext.User.IsInRole("SysAdmin");
        }
    }
}
