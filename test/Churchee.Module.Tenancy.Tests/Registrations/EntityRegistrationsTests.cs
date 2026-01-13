using Churchee.Module.Tenancy.Data.Registrations;
using Churchee.Module.Tenancy.Entities;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Tenancy.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_ApplicationTenant_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(ApplicationTenant));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("ApplicationTenant");
        }

        [Fact]
        public void EntityRegistrationsTests_ApplicationFeature_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(ApplicationFeature));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("ApplicationFeature");
        }

        [Fact]
        public void EntityRegistrationsTests_ApplicationHost_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(ApplicationHost));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("ApplicationHost");
        }

        private static ModelBuilder GetBuilder()
        {
            // Arrange
            var modelBuilder = new ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());
            var cut = new EntityRegistrations();

            // Act
            cut.RegisterEntities(modelBuilder);
            return modelBuilder;
        }
    }

}
