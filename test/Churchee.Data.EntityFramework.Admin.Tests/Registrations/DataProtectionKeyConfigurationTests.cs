using Churchee.Data.EntityFramework.Admin.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Data.EntityFramework.Admin.Tests.Registrations
{
    public class DataProtectionKeyConfigurationTests
    {
        [Fact]
        public void RegisterEntities_ShouldConfigureDataProtectionKeyEntity()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(DataProtectionKey));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("DataProtectionKey");


            // Assert Properties
            eventEntityType?.FindProperty(nameof(DataProtectionKey.Xml))?.GetMaxLength().Should().Be(4000);
        }

        private static ModelBuilder GetBuilder()
        {
            // Arrange
            var modelBuilder = new ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());
            var cut = new DataProtectionKeyConfiguration();

            // Act
            cut.RegisterEntities(modelBuilder);
            return modelBuilder;
        }
    }
}
