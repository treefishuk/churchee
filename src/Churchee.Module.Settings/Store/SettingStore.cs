using Churchee.Common.Storage;
using Churchee.Module.Settings.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Churchee.Module.Settings.Store
{
    public class SettingStore : ISettingStore
    {
        private readonly IDataStore _dataStore;
        private readonly IConfiguration _configuration;

        public SettingStore(IDataStore dataStore, IConfiguration configuration)
        {
            _dataStore = dataStore;
            _configuration = configuration;
        }

        public async Task AddOrUpdateSetting(Guid id, Guid applicationTenantId, string description, string value)
        {
            var setting = await _dataStore.GetRepository<Setting>()
                .GetQueryable()
                .IgnoreQueryFilters()
                .Where(w => w.Id == id && w.ApplicationTenantId == applicationTenantId)
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                await AddSetting(id, applicationTenantId, description, value);
            }

            if (setting != null)
            {
                setting.Value = value;

                await _dataStore.SaveChangesAsync();

            }

        }

        public async Task AddSetting(Guid id, Guid applicationTenantId, string description, string value)
        {
            _dataStore.GetRepository<Setting>().Create(new Setting(id, applicationTenantId, description, value));

            await _dataStore.SaveChangesAsync();
        }

        public async Task ClearSetting(Guid id, Guid applicationTenantId)
        {
            var setting = await _dataStore.GetRepository<Setting>()
                .GetQueryable()
                .IgnoreQueryFilters()
                .Where(w => w.Id == id && w.ApplicationTenantId == applicationTenantId)
                .FirstOrDefaultAsync();

            if (setting == null)
            {
                return;
            }

            _dataStore.GetRepository<Setting>().PermanentDelete(setting);

            await _dataStore.SaveChangesAsync();
        }

        public async Task<string> GetSettingValue(Guid id, Guid applicationTenantId)
        {
            string setting = await _dataStore.GetRepository<Setting>()
                .GetQueryable()
                .IgnoreQueryFilters()
                .Where(x => x.Id == id && x.ApplicationTenantId == applicationTenantId)
                .Select(s => s.Value)
                .FirstOrDefaultAsync() ?? string.Empty;

            if (string.IsNullOrEmpty(setting))
            {
                setting = _configuration[$"Settings:{id}"] ?? string.Empty;
            }

            return setting;
        }
    }
}
