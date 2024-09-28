using Churchee.Common.Extensibility;
using FluentAssertions;

namespace Churchee.Common.Tests.Extensibility
{
    public class MenuItemTests
    {

        [Fact]
        public void MenuItem_WithThreeContructorProperties_SetsCorrecValues()
        {
            //arrange
            string path = "/management/test";

            //act
            var cut = new MenuItem("Test", path, "Icon");

            //assert
            cut.Order.Should().Be(1);
            cut.Path.Should().Be(path);
            cut.Icon.Should().Be("Icon");
            cut.RequiredRole.Should().Be(string.Empty);
            cut.Children.Should().NotBeNull();
            cut.Children.Count.Should().Be(0);
        }

        [Fact]
        public void MenuItem_WithFourContructorProperties_SetsCorrecValues()
        {
            //arrange
            string path = "/management/test";

            //act
            var cut = new MenuItem("Test", path, "Icon", 2);

            //assert
            cut.Order.Should().Be(2);
            cut.Path.Should().Be(path);
            cut.Icon.Should().Be("Icon");
            cut.RequiredRole.Should().Be(string.Empty);
            cut.Children.Should().NotBeNull();
            cut.Children.Count.Should().Be(0);
        }

        [Fact]
        public void MenuItem_WithFiveContructorProperties_SetsCorrecValues()
        {
            //arrange
            string path = "/management/test";
            string reqiredRole = "SysAdmin";

            //act
            var cut = new MenuItem("Test", path, "Icon", 3, reqiredRole);

            //assert
            cut.Order.Should().Be(3);
            cut.Path.Should().Be(path);
            cut.Icon.Should().Be("Icon");
            cut.RequiredRole.Should().Be(reqiredRole);
            cut.Children.Should().NotBeNull();
            cut.Children.Count.Should().Be(0);
        }

        [Fact]
        public void MenuItem_WithMissingPath_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new MenuItem("Test", string.Empty, "Icon"));
        }

        [Fact]
        public void MenuItem_WithMissingName_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new MenuItem(string.Empty, "/management/test", "Icon"));
        }

        [Fact]
        public void MenuItem_WithMissingIcon_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => new MenuItem("Test", "/management/test", string.Empty));
        }


        [Fact]
        public void MenuItem_WithInvalidPath_ThrowsException()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();

            //act
            var exception = Assert.Throws<FormatException>(() => new MenuItem("Test", "Path", "Icon"));

            //assert
            exception.Message.Should().Be("Path must start with /management");

        }


    }
}
