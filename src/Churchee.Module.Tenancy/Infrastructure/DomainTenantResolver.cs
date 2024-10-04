﻿using Churchee.Module.Tenancy.Entities;
using Churchee.Sites.db;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Churchee.Module.Tenancy.Infrastructure
{
    public class DomainTenantResolver : ITenantResolver
    {
        private readonly TenantContext _storage;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _memoryCache;

        public DomainTenantResolver(TenantContext storage, IHttpContextAccessor httpContextAccessor, IMemoryCache memoryCache)
        {
            _storage = storage;
            _httpContextAccessor = httpContextAccessor;
            _memoryCache = memoryCache;
        }

        public Guid GetTenantId()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Guid.Empty;
            }

            string domain = _httpContextAccessor.HttpContext.Request.Host.Host;

            var hasCachedEntry = _memoryCache.TryGetValue($"{domain}_tenantId", out Guid returnValue);

            if (hasCachedEntry && returnValue != Guid.Empty)
            {
                return returnValue;
            }

            returnValue = GetTenantIdFromDB(domain);

            if (returnValue != Guid.Empty)
            {

                _memoryCache.Set($"{domain}_tenantId", returnValue);
            }

            return returnValue;
        }

        public string GetTenantDevName()
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return string.Empty;
            }

            string domain = _httpContextAccessor.HttpContext.Request.Host.Host;

            string? result = _memoryCache.GetOrCreate($"{domain}_tenantName", cachedEntry =>
            {

                cachedEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30);

                return GetTenantDevNameFromDB(domain);

            });

            return result ?? string.Empty;
        }

        private Guid GetTenantIdFromDB(string domain)
        {
            var repo = _storage.Set<ApplicationHost>();

            var tenant = repo.AsNoTracking().Where(w => w.Host == domain).Select(s => new { s.ApplicationTenantId }).FirstOrDefault();

            if (tenant == null)
            {
                return Guid.Empty;
            }

            return tenant.ApplicationTenantId;
        }

        private string GetTenantDevNameFromDB(string domain)
        {
            var repo = _storage.Set<ApplicationHost>();

            string tenantDevname = repo.AsNoTracking().Where(w => w.Host == domain).Select(s => s.ApplicationTenant.DevName).FirstOrDefault() ?? string.Empty;

            return tenantDevname;
        }
    }
}
