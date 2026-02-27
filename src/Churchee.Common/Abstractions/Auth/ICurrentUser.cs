using System;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Auth
{
    public interface ICurrentUser
    {
        string GetUserId();

        Task<Guid> GetApplicationTenantId();

        Task<string> GetApplicationTenantName();

        bool HasFeature(string featureName);

        bool HasRole(string roleName);
    }
}
