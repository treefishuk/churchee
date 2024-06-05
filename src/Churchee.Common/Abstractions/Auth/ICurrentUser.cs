using System;
using System.Threading.Tasks;

namespace Churchee.Common.Abstractions.Auth
{
    public interface ICurrentUser
    {
        Task<Guid> GetApplicationTenantId();

        bool HasFeature(string featureName);

        bool HasRole(string roleName);
    }
}
