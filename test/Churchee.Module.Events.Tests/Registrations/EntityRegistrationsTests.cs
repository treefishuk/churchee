using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Registration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Events.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_EventConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Event));

            // Assert Table Name
            eventEntityType?.GetTableName().Should().Be("Events");


            // Assert Properties
            eventEntityType?.FindProperty(nameof(Event.Latitude))?.GetPrecision().Should().Be(8);
            eventEntityType?.FindProperty(nameof(Event.Latitude))?.GetScale().Should().Be(6);

            eventEntityType?.FindProperty(nameof(Event.Longitude))?.GetPrecision().Should().Be(9);
            eventEntityType?.FindProperty(nameof(Event.Longitude))?.GetScale().Should().Be(6);

            eventEntityType?.FindProperty(nameof(Event.PostCode))?.GetMaxLength().Should().Be(20);

            eventEntityType?.FindProperty(nameof(Event.Content))?.GetColumnType().Should().Be("nvarchar(max)");

        }

        [Fact]
        public void EntityRegistrationsTests_EventDateConfiguredCorrectly()
        {
            var modelBuilder = GetBuilder();

            // Assert
            var eventDateEntityType = modelBuilder.Model.FindEntityType(typeof(EventDate));
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Event));

            // Assert Table Name
            eventDateEntityType?.GetTableName().Should().Be("EventDates");

            // Assert Keys

            var foreignKeys = eventDateEntityType?.GetForeignKeys();

            var entityForeignKey = foreignKeys?.Where(w => w.PrincipalEntityType == eventEntityType);

            entityForeignKey?.SingleOrDefault()?.DeleteBehavior.Should().Be(DeleteBehavior.Cascade);


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
