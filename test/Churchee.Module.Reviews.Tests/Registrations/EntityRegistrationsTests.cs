using Churchee.Module.Reviews.Entities;
using Churchee.Module.Reviews.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Reviews.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_WebContent_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Review));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("Reviews");

            // Assert Properties
            eventEntityType?.FindProperty(nameof(Review.Comment))?.GetColumnType()?.Should().Be("nvarchar(4000)");
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
