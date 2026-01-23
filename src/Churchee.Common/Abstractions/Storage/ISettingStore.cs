using System;
using System.Threading.Tasks;

namespace Churchee.Common.Storage
{
    public interface ISettingStore
    {
        Task ClearSetting(Guid id, Guid applicationTenantId);

        Task AddSetting(Guid id, Guid applicationTenantId, string description, string value);

        Task AddOrUpdateSetting(Guid id, Guid applicationTenantId, string description, string value);

        Task<string> GetSettingValue(Guid id, Guid applicationTenantId);
    }
}
