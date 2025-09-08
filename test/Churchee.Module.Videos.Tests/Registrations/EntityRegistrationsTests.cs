using Churchee.Module.Videos.Entities;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Videos.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void RegisterEntities_ConfiguresVideoEntity()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var EntityTypeVideo = modelBuilder.Model.FindEntityType(typeof(Video));

            // Assert Table Name
            EntityTypeVideo?.GetTableName()?.Should().Be("Videos");

            // Assert VideoUri has an index
            EntityTypeVideo?.GetIndexes().Any(idx => idx.Properties.Any(p => p.Name == nameof(Video.VideoUri))).Should().BeTrue();
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