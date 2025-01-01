using Churchee.Common.Attributes;
using Churchee.Data.EntityFramework.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Churchee.Data.EntityFramework.Tests.Extensions
{
    public class ModelBuilderExtensionsTests
    {
        [Fact]
        public void ApplyGlobalFilters_ShouldApplyFilterToEntitiesImplementingInterface()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            var filterExpression = (Expression<Func<IHasIsActive, bool>>)(e => e.IsActive);

            // Act
            modelBuilder.ApplyGlobalFilters(filterExpression);

            // Assert

            var testEntityEntityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
            var anotherEntityType = modelBuilder.Model.FindEntityType(typeof(AnotherTestEntity));

            testEntityEntityType?.GetQueryFilter().Should().NotBeNull();
            anotherEntityType?.GetQueryFilter().Should().BeNull();
        }

        [Fact]
        public void SetDefaultStringLengths_ShouldSetMaxLengthForStringProperties()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            var filterExpression = (Expression<Func<IHasIsActive, bool>>)(e => e.IsActive);

            // Act
            modelBuilder.SetDefaultStringLengths(50);

            // Assert
            var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntity));
            var property = entityType?.FindProperty(nameof(TestEntity.TestProperty));
            property?.GetMaxLength().Should().Be(50);
        }

        [Fact]
        public void EncryptProtectedProperties_ShouldSetEncryptionConverterForProtectedProperties()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            modelBuilder.EncryptProtectedProperties("0123456789abcdef");

            // Assert
            var entityType = modelBuilder.Model.FindEntityType(typeof(TestEntityWithProtectedProperty));
            var property = entityType?.FindProperty(nameof(TestEntityWithProtectedProperty.ProtectedProperty));
            property?.GetValueConverter().Should().NotBeNull();
        }

        private static ModelBuilder GetBuilder()
        {
            // Arrange
            var modelBuilder = new ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());

            modelBuilder.Entity<TestEntity>(etb =>
            {
                etb.ToTable("TestEntity");

            });

            modelBuilder.Entity<TestEntity>(etb =>
            {
                etb.ToTable("AnotherTestEntity");

            });

            modelBuilder.Entity<TestEntityWithProtectedProperty>(etb =>
            {
                etb.ToTable("TestEntityWithProtectedProperty");

            });

            return modelBuilder;
        }


        public class TestEntity : IHasIsActive
        {
            public int Id { get; set; }
            public string TestProperty { get; set; }
            public bool IsActive { get; set; }
        }

        public class AnotherTestEntity
        {
            public int Id { get; set; }
            public string TestProperty { get; set; }
        }

        public class TestEntityWithProtectedProperty
        {
            public int Id { get; set; }

            [EncryptProperty]
            public string ProtectedProperty { get; set; }
        }

        public interface IHasIsActive
        {
            bool IsActive { get; set; }
        }
    }
}
