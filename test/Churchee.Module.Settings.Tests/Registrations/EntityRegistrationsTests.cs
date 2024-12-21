using Churchee.Module.Settings.Entities;
using Churchee.Module.Settings.Registration;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Settings.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_SettingConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var settingEntityType = modelBuilder.Model.FindEntityType(typeof(Setting));

            // Assert Table Name
            settingEntityType?.GetTableName().Should().Be("Settings");

            // Assert Properties
            settingEntityType?.FindProperty(nameof(Setting.Id))?.IsKey().Should().BeTrue();
            settingEntityType?.FindProperty(nameof(Setting.Description))?.GetMaxLength().Should().Be(256);
            settingEntityType?.FindProperty(nameof(Setting.Value))?.GetMaxLength().Should().Be(256);
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
