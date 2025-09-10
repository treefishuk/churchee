using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageType;

public class CreatePageTypeCommandTests
{
    [Fact]
    public void Constructor_SetsName()
    {
        // Arrange
        var name = "TestPageType";

        // Act
        var command = new CreatePageTypeCommand(name);

        // Assert
        Assert.Equal(name, command.Name);
    }
}
