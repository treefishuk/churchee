using Churchee.Module.Site.Features.PageTypes.Commands;

namespace Churchee.Module.Site.Tests.Features.PageTypes.Commands.DeletePageType
{
    public class DeletePageTypeCommandTests
    {
        [Fact]
        public void Constructor_SetsId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var command = new DeletePageTypeCommand(id);

            // Assert
            Assert.Equal(id, command.Id);
        }
    }
}
