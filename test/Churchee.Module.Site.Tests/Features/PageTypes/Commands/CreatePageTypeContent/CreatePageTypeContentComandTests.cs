using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent;

public class CreatePageTypeContentComandTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        var pageTypeId = Guid.NewGuid();
        var name = "ContentName";
        var type = "string";
        var required = true;
        var order = 2;

        // Act
        var command = new CreatePageTypeContentComand(pageTypeId, name, type, required, order);

        // Assert
        Assert.Equal(pageTypeId, command.PageTypeId);
        Assert.Equal(name, command.Name);
        Assert.Equal(type, command.Type);
        Assert.Equal(required, command.Required);
        Assert.Equal(order, command.Order);
    }
}
