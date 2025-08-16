using Ardalis.Specification;
using Churchee.Common.Abstractions.Entities;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Settings.Entities;
using Churchee.Module.Settings.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;

namespace Churchee.Module.Settings.Tests.Store
{
    public class SettingStoreTests
    {
        private readonly DbContextOptions<DataStoreContext> _dbContextOptions;
        private readonly IConfiguration _configuration;

        public SettingStoreTests()
        {
            // Configure in-memory database
            _dbContextOptions = new DbContextOptionsBuilder<DataStoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Mock configuration
            var inMemorySettings = new System.Collections.Generic.Dictionary<string, string?>
            {
                { "Settings:TestSettingId", "Config Value" }
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        [Fact]
        public async Task AddOrUpdateSetting_ShouldAddNewSetting_WhenSettingDoesNotExist()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            string description = "Test Description";
            string value = "Test Value";

            // Act
            await settingStore.AddOrUpdateSetting(id, tenantId, description, value);

            // Assert
            var setting = context.Settings.FirstOrDefault(s => s.Id == id && s.ApplicationTenantId == tenantId);
            Assert.NotNull(setting);
            Assert.Equal(description, setting.Description);
            Assert.Equal(value, setting.Value);
        }

        [Fact]
        public async Task AddOrUpdateSetting_ShouldUpdateExistingSetting_WhenSettingExists()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            string description = "Test Description";
            string oldValue = "Old Value";
            string newValue = "Updated Value";

            context.Settings.Add(new Setting(id, tenantId, description, oldValue));
            await context.SaveChangesAsync();

            // Act
            await settingStore.AddOrUpdateSetting(id, tenantId, description, newValue);

            // Assert
            var setting = context.Settings.FirstOrDefault(s => s.Id == id && s.ApplicationTenantId == tenantId);
            Assert.NotNull(setting);
            Assert.Equal(newValue, setting.Value);
        }

        [Fact]
        public async Task ClearSetting_ShouldDeleteSetting_WhenSettingExists()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            string description = "Test Description";
            string value = "Test Value";

            context.Settings.Add(new Setting(id, tenantId, description, value));
            await context.SaveChangesAsync();

            // Act
            await settingStore.ClearSetting(id, tenantId);

            // Assert
            var setting = context.Settings.FirstOrDefault(s => s.Id == id && s.ApplicationTenantId == tenantId);
            Assert.Null(setting);
        }

        [Fact]
        public async Task GetSettingValue_ShouldReturnSettingValue_WhenSettingExists()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            string value = "Test Value";

            context.Settings.Add(new Setting(id, tenantId, "Test Description", value));
            await context.SaveChangesAsync();

            // Act
            string result = await settingStore.GetSettingValue(id, tenantId);

            // Assert
            Assert.Equal(value, result);
        }

        [Fact]
        public async Task GetSettingValue_ShouldReturnConfigurationValue_WhenSettingDoesNotExist()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.Parse("438406d8-fd41-4829-9b28-3bc1050c6335");
            var tenantId = Guid.NewGuid();

            context.Settings.Add(new Setting(id, tenantId, "Config Desc", "Config Value"));

            await context.SaveChangesAsync();

            // Act
            string result = await settingStore.GetSettingValue(id, tenantId);

            // Assert
            Assert.Equal("Config Value", result);
        }

        [Fact]
        public async Task GetSettingValue_ShouldReturnEmptyString_WhenSettingAndConfigurationDoNotExist()
        {
            // Arrange
            using var context = new DataStoreContext(_dbContextOptions);
            var settingStore = new SettingStore(new DataStore(context), _configuration);

            var id = Guid.NewGuid();
            var tenantId = Guid.NewGuid();

            // Act
            string result = await settingStore.GetSettingValue(id, tenantId);

            // Assert
            Assert.Equal(string.Empty, result);
        }
    }

    // In-memory DbContext for testing
    public class DataStoreContext : DbContext
    {
        public DataStoreContext(DbContextOptions<DataStoreContext> options) : base(options) { }

        public DbSet<Setting> Settings { get; set; }
    }

    // Concrete implementation of IDataStore for testing
    public class DataStore : IDataStore
    {
        private readonly DataStoreContext _context;

        public DataStore(DataStoreContext context)
        {
            _context = context;
        }

        public IRepository<T> GetRepository<T>() where T : class, IEntity
        {
            return new Repository<T>(_context);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Concrete implementation of IRepository for testing
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        private readonly DbSet<T> _dbSet;

        public Repository(DataStoreContext context)
        {
            _dbSet = context.Set<T>();
        }

        public IQueryable<T> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public void Create(T entity)
        {
            _dbSet.Add(entity);
        }

        public void PermanentDelete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public IQueryable<T> ApplySpecification(ISpecification<T> specification, bool evaluateCriteriaOnly = false)
        {
            throw new NotImplementedException();
        }

        public Task<T> FirstOrDefaultAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> FirstOrDefaultAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public T GetById(params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<T> GetByIdAsync(object id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task PermanentDelete(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task SoftDelete<TId>(TId id)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public bool Any()
        {
            throw new NotImplementedException();
        }

        public bool AnyWithFiltersDisabled(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public int Count()
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetDistinctCountAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetListAsync(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> GetListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        public Task<List<TResult>> GetListAsync<TKey, TResult>(ISpecification<T> specification, Expression<Func<T, TKey>> groupBy, Expression<Func<IGrouping<TKey, T>, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> GetListAsync<TKey, TResult>(ISpecification<T> specification, Expression<Func<T, TKey>> groupBy, Expression<Func<IGrouping<TKey, T>, TResult>> selector, int take, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Common.Abstractions.DataTableResponse<TResult>> GetDataTableResponseAsync<TResult>(ISpecification<T> specification, string orderBy, string orderByDir, int skip, int take, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<List<TResult>> GetDistinctListAsync<TResult>(ISpecification<T> specification, Expression<Func<T, TResult>> selector, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task PermanentDelete(ISpecification<T> specification, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SoftDelete(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
