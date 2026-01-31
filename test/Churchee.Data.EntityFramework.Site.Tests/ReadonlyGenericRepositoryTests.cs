using Ardalis.Specification;
using Churchee.Common.Abstractions.Entities;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Site.Tests
{
    public class ReadonlyGenericRepositoryTests
    {
        private readonly TestDbContext _dbContext;
        private readonly ReadonlyGenericRepository<TestEntity> _repository;

        public ReadonlyGenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new TestDbContext(options);
            _repository = new ReadonlyGenericRepository<TestEntity>(_dbContext);
        }

        [Fact]
        public async Task Create_Should_Throw_Exception()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

            // Act
            Action act = () => _repository.Create(entity);

            // Assert
            act.Should().Throw<InvalidOperationException>();
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
        public void Update_Should_Throw_Exception()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

            // Act
            Action act = () => _repository.Update(entity);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public async Task PermanentDelete_Should_Throw_Exception()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

            // Act
            Action act = () => _repository.PermanentDelete(entity);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }


        [Fact]
        public async Task SoftDelete_Should_Throw_Exception()
        {
            // Arrange
            var entity = new TestEntity { Id = Guid.NewGuid(), Name = "Test Entity" };

            // Act
            var act = async () => await _repository.SoftDelete(entity.Id);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
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