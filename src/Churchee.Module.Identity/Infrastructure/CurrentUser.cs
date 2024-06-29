using Churchee.Common.Abstractions.Auth;
using System;
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
            string id = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(id);

            var claims = await _userManager.GetClaimsAsync(user);

            var claim = claims.Where(w => w.Type == "ActiveTenantId").FirstOrDefault();

            if (claim != null)
            {
                return Guid.Parse(claim.Value);
            }

            return Guid.Empty;
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
