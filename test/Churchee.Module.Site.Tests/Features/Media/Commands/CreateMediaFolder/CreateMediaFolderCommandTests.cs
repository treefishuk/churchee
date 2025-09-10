using Churchee.Module.Site.Features.Media.Commands;

public class CreateMediaFolderCommandTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        Guid? parentId = Guid.NewGuid();
        string name = "FolderName";

        // Act
        var command = new CreateMediaFolderCommand(parentId, name);

        // Assert
        Assert.Equal(parentId, command.ParentId);
        Assert.Equal(name, command.Name);
    }

    [Fact]
    public void CanSetAndGetProperties()
    {
        // Arrange
        var command = new CreateMediaFolderCommand(null, null);
        Guid? parentId = Guid.NewGuid();
        string name = "AnotherFolder";

        // Act
        command.ParentId = parentId;
        command.Name = name;

        // Assert
        Assert.Equal(parentId, command.ParentId);
        Assert.Equal(name, command.Name);
    }
}
