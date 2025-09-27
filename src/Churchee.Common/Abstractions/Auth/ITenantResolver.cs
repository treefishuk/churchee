using System;

namespace Churchee.Common.Abstractions.Auth
{
    public interface ITenantResolver
    {
        Guid GetTenantId();

        string GetTenantDevName();

        string GetCDNPrefix();
    }
}
