using Churchee.Module.Site.Features.Pages.Commands;

public class DeletePageCommandTests
{
    [Fact]
    public void Constructor_SetsPageId()
    {
        // Arrange
        var pageId = Guid.NewGuid();

        // Act
        var command = new DeletePageCommand(pageId);

        // Assert
        Assert.Equal(pageId, command.PageId);
    }
}
