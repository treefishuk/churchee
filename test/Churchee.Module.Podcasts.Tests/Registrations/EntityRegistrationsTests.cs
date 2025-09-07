using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Podcasts.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_PodcastConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Podcast));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("Podcasts");


            // Assert Properties
            eventEntityType?.FindProperty(nameof(Podcast.Content))?.GetColumnType()?.Should().Be("nvarchar(max)");
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
