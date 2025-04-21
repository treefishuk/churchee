using Churchee.Common.Abstractions.Entities;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Tests
{
    public class GenericRepositoryTests
    {
        private readonly TestDbContext _dbContext;
        private readonly GenericRepository<TestEntity> _repository;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TestDbContext(options);
            _repository = new GenericRepository<TestEntity>(_dbContext);
        }

        [Fact]
        public void Create_ShouldAddEntityToDbSet()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

            // Act
            _repository.Create(entity);
            _dbContext.SaveChanges();

            // Assert
            Assert.Single(_dbContext.TestEntities);
            Assert.Equal("Test Entity", _dbContext.TestEntities.First().Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnEntityById()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Name, result.Name);
        }

        [Fact]
        public void Update_ShouldAttachEntityToDbContext()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            _dbContext.SaveChanges();

            // Act
            entity.Name = "Updated Entity";
            _repository.Update(entity);
            _dbContext.SaveChanges();

            // Assert
            Assert.Equal("Updated Entity", _dbContext.TestEntities.First().Name);
        }

        [Fact]
        public void Any_ShouldReturnTrueIfEntitiesExist()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Any();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task PermanentDelete_ShouldRemoveEntityFromDbSet()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.PermanentDelete(entity.Id);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.Empty(_dbContext.TestEntities);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnEntityCount()
        {
            // Arrange
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" });
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            await _dbContext.SaveChangesAsync();

            // Act
            var count = await _repository.CountAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
        }
    }

    // Test Entity
    public class TestEntity : IEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
    }

    // Test DbContext
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

        public DbSet<TestEntity> TestEntities { get; set; }
    }
}
