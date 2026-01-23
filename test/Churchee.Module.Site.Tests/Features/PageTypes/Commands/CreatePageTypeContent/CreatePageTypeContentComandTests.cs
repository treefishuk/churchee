using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent;

namespace Churchee.Module.Site.Tests.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentComandTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var pageTypeId = Guid.NewGuid();
            string name = "ContentName";
            string type = "string";
            bool required = true;
            int order = 2;

            // Act
            var command = new CreatePageTypeContentCommand(pageTypeId, name, type, required, order);

            // Assert
            Assert.Equal(pageTypeId, command.PageTypeId);
            Assert.Equal(name, command.Name);
            Assert.Equal(type, command.Type);
            Assert.Equal(required, command.Required);
            Assert.Equal(order, command.Order);
        }
    }
}
