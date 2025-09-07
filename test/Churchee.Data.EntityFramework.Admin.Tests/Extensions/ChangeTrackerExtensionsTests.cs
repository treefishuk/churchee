using Churchee.Data.EntityFramework.Admin.Extensions;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Churchee.Data.EntityFramework.Admin.Tests.Extensions
{
    public class ChangeTrackerExtensionsTests
    {
        [Fact]
        public void ApplyTrimOnStringFields_ShouldTrimStringProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new TestDbContext(options);
            var entity = new TestEntity { TestProperty = "  test  " };
            context.TestEntities.Add(entity);
            context.SaveChanges();

            var entities = context.ChangeTracker.Entries().ToList();

            // Act
            entities.ApplyTrimOnStringFields();

            // Assert
            entity.TestProperty.Should().Be("test");
        }

        [Fact]
        public void ApplyTrimOnStringFields_ShouldNotTrimNonStringProperties()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new TestDbContext(options);
            var entity = new TestEntityWithNonStringProperty { TestProperty = 123 };
            context.TestEntitiesWithNonStringProperty.Add(entity);
            context.SaveChanges();

            var entities = context.ChangeTracker.Entries().ToList();

            // Act
            entities.ApplyTrimOnStringFields();

            // Assert
            entity.TestProperty.Should().Be(123);
        }

        [Fact]
        public void ApplyTrimOnStringFields_ShouldTrimStringPropertiesWithMaxLength()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new TestDbContext(options);
            var entity = new TestEntityWithMaxLength { TestProperty = "  test string  " };
            context.TestEntitiesWithMaxLength.Add(entity);
            context.SaveChanges();

            var entities = context.ChangeTracker.Entries().ToList();

            // Act
            entities.ApplyTrimOnStringFields();

            // Assert
            entity.TestProperty.Should().Be("test");
        }


        [Fact]
        public void ApplyTrimOnStringFields_ShouldHandleNullValues()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using var context = new TestDbContext(options);
            var entity = new TestEntity { TestProperty = null };
            context.TestEntities.Add(entity);
            context.SaveChanges();

            var entities = context.ChangeTracker.Entries().ToList();

            // Act
            entities.ApplyTrimOnStringFields();

            // Assert
            entity.TestProperty.Should().BeNull();
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            public DbSet<TestEntity> TestEntities { get; set; }
            public DbSet<TestEntityWithMaxLength> TestEntitiesWithMaxLength { get; set; }
            public DbSet<TestEntityWithNonStringProperty> TestEntitiesWithNonStringProperty { get; set; }
        }

        public class TestEntity
        {
            public int Id { get; set; }
            public string? TestProperty { get; set; }
        }

        public class TestEntityWithMaxLength
        {
            public int Id { get; set; }
            [MaxLength(4)]
            public string? TestProperty { get; set; }
        }

        public class TestEntityWithNonStringProperty
        {
            public int Id { get; set; }
            public int TestProperty { get; set; }
        }
    }
}
