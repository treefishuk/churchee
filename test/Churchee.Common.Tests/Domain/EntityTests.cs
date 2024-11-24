using Churchee.Common.Data;
using FluentAssertions;
using Moq;

namespace Churchee.Common.Tests.Domain
{
    public class EntityTests
    {
        [Fact]
        public void Entity_WithApplicationIdConstructor_SetsPropertiesCorrectly()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();

            //act
            var cut = new Mock<Entity>(applicationTenantId).Object;

            //assert
            cut.ApplicationTenantId.Should().Be(applicationTenantId);
            cut.Id.Should().NotBeEmpty();
            cut.CreatedDate.Should().NotBeNull();
            cut.ModifiedDate.Should().NotBeNull();
            cut.CreatedById.Should().Be(Guid.Empty);
            cut.ModifiedById.Should().Be(Guid.Empty);
            cut.Deleted.Should().Be(false);
            cut.ModifiedByName.Should().BeNull();
            cut.CreatedByUser.Should().BeNull();
        }

        [Fact]
        public void Entity_WithIdAndApplicationIdConstructor_SetsPropertiesCorrectly()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();
            var id = Guid.NewGuid();

            //act
            var cut = new Mock<Entity>(id, applicationTenantId).Object;

            //assert
            cut.ApplicationTenantId.Should().Be(applicationTenantId);
            cut.Id.Should().Be(id);
            cut.CreatedDate.Should().NotBeNull();
            cut.ModifiedDate.Should().NotBeNull();
            cut.CreatedById.Should().Be(Guid.Empty);
            cut.ModifiedById.Should().Be(Guid.Empty);
            cut.Deleted.Should().Be(false);
            cut.ModifiedByName.Should().BeNull();
            cut.CreatedByUser.Should().BeNull();
        }
    }
}

