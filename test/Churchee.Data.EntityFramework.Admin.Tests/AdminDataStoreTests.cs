using Churchee.Common.Abstractions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace Churchee.Data.EntityFramework.Admin.Tests
{
    public class AdminDataStoreTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;

        public AdminDataStoreTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Fact]
        public void GetRepository_ShouldReturnGenericRepository()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new AdminDataStore(dbContext, _mockHttpContextAccessor.Object);

            // Act
            var repository = efStorage.GetRepository<TestTrackableEntity>();

            // Assert
            Assert.NotNull(repository);
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldCallSaveChangesOnDbContext()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new AdminDataStore(dbContext, _mockHttpContextAccessor.Object);

            var trackableEntity = new TestTrackableEntity();
            dbContext.Add(trackableEntity);

            // Act
            int result = await efStorage.SaveChangesAsync();

            // Assert
            Assert.Equal(1, result); // Ensure one change was saved to the database
            Assert.NotNull(dbContext.TestEntities.FirstOrDefault()); // Ensure the entity was saved
        }

        [Fact]
        public void SaveChanges_ShouldCallSaveChangesOnDbContext()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new AdminDataStore(dbContext, _mockHttpContextAccessor.Object);

            var trackableEntity = new TestTrackableEntity();
            dbContext.Add(trackableEntity);

            // Act
            efStorage.SaveChanges();

            // Assert
            Assert.NotNull(dbContext.TestEntities.FirstOrDefault()); // Ensure the entity was saved
        }

        [Fact]
        public async Task SaveChangesAsync_ShouldAssignAutoValues_WhenEntityIsTrackable()
        {
            // Arrange
            var dbContext = new TestDbContext();

            var efStorage = new AdminDataStore(dbContext, _mockHttpContextAccessor.Object);

            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString())
            }, "mock"));

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpContext.Setup(ctx => ctx.User).Returns(userClaims);
            _mockHttpContextAccessor.Setup(accessor => accessor.HttpContext).Returns(mockHttpContext.Object);

            // Act
            var trackableEntity = new TestTrackableEntity();
            dbContext.Add(trackableEntity);

            int result = await efStorage.SaveChangesAsync();

            // Assert
            Assert.NotNull(trackableEntity.CreatedDate);
            Assert.Equal("TestUser", trackableEntity.CreatedByUser);
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
