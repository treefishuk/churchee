using Churchee.Module.Settings.Entities;
using FluentAssertions;

namespace Churchee.Module.Settings.Tests.Entities
{
    public class SettingTests
    {
        [Fact]
        public void Setting_ShouldInitializeProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();
            var description = "Test Description";
            var value = "Test Value";

            // Act
            var setting = new Setting(id, applicationTenantId, description, value);

            // Assert
            setting.Id.Should().Be(id);
            setting.ApplicationTenantId.Should().Be(applicationTenantId);
            setting.Description.Should().Be(description);
            setting.Value.Should().Be(value);
        }

        [Fact]
        public void Setting_ShouldAllowUpdatingDescription()
        {
            // Arrange
            var setting = new Setting(Guid.NewGuid(), Guid.NewGuid(), "Initial Description", "Initial Value");
            var newDescription = "Updated Description";

            // Act
            setting.Description = newDescription;

            // Assert
            setting.Description.Should().Be(newDescription);
        }

        [Fact]
        public void Setting_ShouldAllowUpdatingValue()
        {
            // Arrange
            var setting = new Setting(Guid.NewGuid(), Guid.NewGuid(), "Initial Description", "Initial Value");
            var newValue = "Updated Value";

            // Act
            setting.Value = newValue;

            // Assert
            setting.Value.Should().Be(newValue);
        }
    }
}
