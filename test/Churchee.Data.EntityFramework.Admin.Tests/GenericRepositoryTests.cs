using Ardalis.Specification;
using Churchee.Common.Abstractions.Entities;
using Churchee.Data.EntityFramework.Shared;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Admin.Tests
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
        public async Task GetByIdAsync_WithCancelationToken_ShouldReturnEntityById()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(entity.Id, default);

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
        public async Task PermanentDelete_UsingEntity_ShouldRemoveEntityFromDbSet()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            _repository.PermanentDelete(entity);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.Empty(_dbContext.TestEntities);
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
        public async Task SoftDelete_ShouldMarkAsTrue()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            await _dbContext.SaveChangesAsync();

            // Act
            await _repository.SoftDelete(entity.Id);
            await _dbContext.SaveChangesAsync();

            // Assert
            Assert.True(entity.Deleted);
        }


        [Fact]
        public void Count_ShouldReturnEntityCount()
        {
            // Arrange
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" });
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            _dbContext.SaveChanges();

            // Act
            int count = _repository.Count();

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task CountAsync_ShouldReturnEntityCount()
        {
            // Arrange
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" });
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            await _dbContext.SaveChangesAsync();

            // Act
            int count = await _repository.CountAsync(CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public void Any_ShouldReturnTrueIfEntitiesExist()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            _dbContext.SaveChanges();

            // Act
            bool result = _repository.Any();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AnyWithFiltersDisabled_ShouldReturnCorrectCountCount()
        {
            // Arrange
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1", Deleted = true });
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2", Deleted = true });
            await _dbContext.SaveChangesAsync();

            // Act
            bool exists = _repository.AnyWithFiltersDisabled(a => a.Name != null);

            // Assert
            Assert.True(exists);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_ShouldReturnEntity_WhenSpecificationMatches()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.Set<TestEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name == "Test Entity");

            // Act
            var result = await _repository.FirstOrDefaultAsync(specification, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_WithSelector_ShouldReturnProjectedResult_WhenSpecificationMatches()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.Set<TestEntity>().Add(entity);
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name == "Test Entity");

            // Act
            string result = await _repository.FirstOrDefaultAsync(specification, e => e.Name, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Entity", result);
        }

        [Fact]
        public async Task GetListAsync_ShouldReturnEntities_WhenSpecificationMatches()
        {
            // Arrange
            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" }
            );
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = await _repository.GetListAsync(specification, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetListAsync_WithSelector_ShouldReturnProjectedResults_WhenSpecificationMatches()
        {
            // Arrange
            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" }
            );
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = await _repository.GetListAsync(specification, e => e.Name, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains("Entity 1", result);
            Assert.Contains("Entity 2", result);
        }

        [Fact]
        public async Task GetDataTableResponseAsync_ShouldReturnPaginatedResults()
        {
            // Arrange
            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" }
            );
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act

            var result = await _repository.GetDataTableResponseAsync(
                specification: specification,
                orderBy: "Name",
                orderByDir: "asc",
                skip: 0,
                take: 2,
                selector: s => new
                {
                    s.Id,
                    s.Name,
                },
                cancellationToken: CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.RecordsTotal);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public void ApplySpecification_ShouldReturnFilteredQueryable_WhenSpecificationMatches()
        {
            // Arrange
            _dbContext.Set<TestEntity>().AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" }
            );

            _dbContext.SaveChanges();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = _repository.ApplySpecification(specification);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void AddRange_ShouldAddMultipleEntities()
        {
            // Arrange
            var entities = new[]
            {
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" }
            };

            // Act
            _repository.AddRange(entities);
            _dbContext.SaveChanges();

            // Assert
            Assert.Equal(2, _dbContext.TestEntities.Count());
        }

        [Fact]
        public void GetById_ShouldReturnEntity_WhenItExists()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };
            _dbContext.TestEntities.Add(entity);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.GetById(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
        }

        [Fact]
        public void GetQueryable_ShouldReturnAllEntities()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            _dbContext.SaveChanges();

            // Act
            var result = _repository.GetQueryable();

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task CountAsync_WithSpecification_ShouldReturnMatches()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Other" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var count = await _repository.CountAsync(specification, CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetDistinctCountAsync_ShouldReturnDistinctProjectedCount()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var count = await _repository.GetDistinctCountAsync(specification, e => e.Name, CancellationToken.None);

            // Assert
            Assert.Equal(2, count);
        }

        [Fact]
        public async Task GetListAsync_WithGroupBySelector_ShouldReturnGroupedResults()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = await _repository.GetListAsync(
                specification,
                e => e.Name,
                g => new { Key = g.Key, Count = g.Count() },
                CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Key == "Entity 1" && x.Count == 2);
            Assert.Contains(result, x => x.Key == "Entity 2" && x.Count == 1);
        }

        [Fact]
        public async Task GetListAsync_WithGroupBySelectorAndTake_ShouldReturnLimitedGroupedResults()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = await _repository.GetListAsync(
                specification,
                e => e.Name,
                g => new { Key = g.Key, Count = g.Count() },
                1,
                CancellationToken.None);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetDistinctListAsync_ShouldReturnDistinctNames()
        {
            // Arrange
            _dbContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            // Act
            var result = await _repository.GetDistinctListAsync(specification, e => e.Name, CancellationToken.None);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains("Entity 1", result);
            Assert.Contains("Entity 2", result);
        }

        [Fact]
        public async Task AnyAsync_ShouldReturnTrue_WhenSpecificationMatches()
        {
            // Arrange
            _dbContext.TestEntities.Add(new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" });
            await _dbContext.SaveChangesAsync();

            var specification = new MockSpecification<TestEntity>(e => e.Name == "Entity 1");

            // Act
            var result = await _repository.AnyAsync(specification, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task BulkInsertOrUpdateAsync_WithEmptyCollection_ShouldReturnWithoutThrowing()
        {
            // Act
            var exception = await Record.ExceptionAsync(() => _repository.BulkInsertOrUpdateAsync(
                new List<TestEntity>(),
                new List<string> { nameof(TestEntity.Name) },
                CancellationToken.None));

            // Assert
            Assert.Null(exception);
        }

        [Fact]
        public async Task PermanentDelete_WithSpecification_ShouldDeleteMatchingRows()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var sqliteContext = new TestDbContext(options);
            await sqliteContext.Database.EnsureCreatedAsync();

            sqliteContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Other" });
            await sqliteContext.SaveChangesAsync();

            var repository = new GenericRepository<TestEntity>(sqliteContext);
            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            await repository.PermanentDelete(specification, CancellationToken.None);

            Assert.Equal(1, await sqliteContext.TestEntities.CountAsync());
            Assert.Equal("Other", (await sqliteContext.TestEntities.SingleAsync()).Name);
        }

        [Fact]
        public async Task PermanentDeleteBatchAsync_WithSpecification_ShouldDeleteOnlyMatchingBatch()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            await using var sqliteContext = new TestDbContext(options);
            await sqliteContext.Database.EnsureCreatedAsync();

            sqliteContext.TestEntities.AddRange(
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 1" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 2" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Entity 3" },
                new TestEntity { Id = Guid.NewGuid(), Name = "Other" });
            await sqliteContext.SaveChangesAsync();

            var repository = new GenericRepository<TestEntity>(sqliteContext);
            var specification = new MockSpecification<TestEntity>(e => e.Name.Contains("Entity"));

            var deletedCount = await repository.PermanentDeleteBatchAsync(specification, 2, CancellationToken.None);

            Assert.Equal(2, deletedCount);
            Assert.Equal(2, await sqliteContext.TestEntities.CountAsync());
            Assert.Contains(await sqliteContext.TestEntities.ToListAsync(), x => x.Name == "Other");
        }

        // Mock Specification
        private class MockSpecification<T> : Specification<T>
        {
            public MockSpecification(Expression<Func<T, bool>> criteria)
            {
                Query.Where(criteria);
            }
        }

        // Test Entity
        public class TestEntity : IEntity
        {
            public TestEntity()
            {
                Name = string.Empty;
            }

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
}