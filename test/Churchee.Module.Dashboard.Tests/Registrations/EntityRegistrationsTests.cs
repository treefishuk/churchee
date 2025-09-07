using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Dashboard.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_EventConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageView));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageViews");


            // Assert Properties
            eventEntityType?.FindProperty(nameof(PageView.Id))?.IsKey().Should().BeTrue();
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
