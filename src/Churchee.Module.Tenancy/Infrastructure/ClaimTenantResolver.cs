﻿using Microsoft.AspNetCore.Http;

namespace Churchee.Module.Tenancy.Infrastructure
{
    public class ClaimTenantResolver : ITenantResolver
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClaimTenantResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid GetTenantId()
        {
            if(_httpContextAccessor.HttpContext == null)
            {
                return Guid.Empty;
            }

            var claim = _httpContextAccessor.HttpContext.User.Claims.Where(w => w.Type == "ActiveTenantId").FirstOrDefault();

            if(claim == null)
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

            var claim = _httpContextAccessor.HttpContext.User.Claims.Where(w => w.Type == "ActiveTenantName").FirstOrDefault();

            if (claim == null)
            {
                return string.Empty;
            }

            return claim.Value;
        }

    }
}
