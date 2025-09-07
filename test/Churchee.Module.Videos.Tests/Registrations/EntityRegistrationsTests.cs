using Microsoft.EntityFrameworkCore;
using Moq;

namespace Churchee.Module.Videos.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void RegisterEntities_ConfiguresVideoEntity()
        {
            // Arrange
            var modelBuilderMock = new Mock<ModelBuilder>(new object[] { new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet() }) { CallBase = true };
            var entityRegistrations = new EntityRegistrations();

            // Act & Assert (should not throw)
            entityRegistrations.RegisterEntities(modelBuilderMock.Object);
        }
    }
}
