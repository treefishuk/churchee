using Churchee.Common.Abstractions.Entities;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Churchee.Data.EntityFramework.Site.Tests
{
    public class SiteDataStoreTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public SiteDataStoreTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void GetRepository_ShouldReturnGenericRepository()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new SiteDataStore(dbContext);

            // Act
            var repository = efStorage.GetRepository<TestTrackableEntity>();

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task SaveChangesAsync_Should_Throw_Exception()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new SiteDataStore(dbContext);

            var trackableEntity = new TestTrackableEntity();
            dbContext.Add(trackableEntity);

            // Act
            var act = async () => await efStorage.SaveChangesAsync();

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();

        }

        [Fact]
        public void SaveChanges_Should_Throw_Exception()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new SiteDataStore(dbContext);

            var trackableEntity = new TestTrackableEntity();
            dbContext.Add(trackableEntity);

            // Act
            var act = efStorage.SaveChanges;

            // Assert
            act.Should().Throw<InvalidOperationException>();

        }

        private class TestTrackableEntity : ITrackable, IEntity
        {
            public TestTrackableEntity()
            {
                CreatedByUser = string.Empty;
                ModifiedByName = string.Empty;
            }

            public Guid Id { get; set; }
            public DateTime? CreatedDate { get; set; }
            public string CreatedByUser { get; set; }
            public Guid? CreatedById { get; set; }
            public DateTime? ModifiedDate { get; set; }
            public string ModifiedByName { get; set; }
            public Guid? ModifiedById { get; set; }
            public bool Deleted { get; set; }
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext() : base(
                new DbContextOptionsBuilder<TestDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options
                )
            {
            }

            public DbSet<TestTrackableEntity> TestEntities { get; set; }
        }

    }
}
