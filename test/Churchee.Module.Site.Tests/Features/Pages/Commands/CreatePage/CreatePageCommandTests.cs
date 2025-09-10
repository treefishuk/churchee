using System;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using Xunit;

public class CreatePageCommandTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly_WithParentId()
    {
        // Arrange
        var title = "Test Title";
        var description = "Test Description";
        var pageTypeId = Guid.NewGuid().ToString();
        var parentId = Guid.NewGuid().ToString();

        // Act
        var command = new CreatePageCommand(title, description, pageTypeId, parentId);

        // Assert
        Assert.Equal(title, command.Title);
        Assert.Equal(description, command.Description);
        Assert.Equal(Guid.Parse(pageTypeId), command.PageTypeId);
        Assert.Equal(Guid.Parse(parentId), command.ParentId);
    }

    [Fact]
    public void Constructor_SetsParentIdToNull_WhenParentIdIsEmpty()
    {
        // Arrange
        var title = "Test Title";
        var description = "Test Description";
        var pageTypeId = Guid.NewGuid().ToString();
        string parentId = "";

        // Act
        var command = new CreatePageCommand(title, description, pageTypeId, parentId);

        // Assert
        Assert.Null(command.ParentId);
    }
}
