using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Registrations;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Churchee.Module.Tokens.Test.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_TokenConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var tokenEntityType = modelBuilder.Model.FindEntityType(typeof(Token));

            // Assert Table Name
            tokenEntityType?.GetTableName().Should().Be("Tokens");

            // Assert Properties
            tokenEntityType?.FindProperty(nameof(Token.Key))?.GetMaxLength().Should().Be(100);
            tokenEntityType?.FindProperty(nameof(Token.Value))?.GetMaxLength().Should().Be(2000);
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
