using Churchee.Module.Identity.Entities;
using FluentAssertions;

namespace Churchee.Module.Identity.Tests.Entities
{
    public class ApplicationRoleTests
    {
        [Fact]
        public void ApplicationRole_IdAndNameContructor_SetsProperties()
        {
            //arrange
            var id = Guid.NewGuid();
            string name = "Dev";

            //act
            var cut = new ApplicationRole(id, name);

            //assert
            cut.Id.Should().Be(id);
            cut.Name.Should().Be(name);
            cut.Selectable.Should().BeFalse();
        }

        [Fact]
        public void ApplicationRole_IdNameAndSelectableContructor_SetsProperties()
        {
            //arrange
            var id = Guid.NewGuid();
            string name = "Dev";
            bool selectable = true;

            //act
            var cut = new ApplicationRole(id, name, selectable);

            //assert
            cut.Id.Should().Be(id);
            cut.Name.Should().Be(name);
            cut.Selectable.Should().BeTrue();
        }

    }
}
