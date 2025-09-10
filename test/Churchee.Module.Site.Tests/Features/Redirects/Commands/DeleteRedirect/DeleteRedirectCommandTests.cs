using Churchee.Module.Site.Features.Redirects.Commands;

public class DeleteRedirectCommandTests
{
    [Fact]
    public void Constructor_SetsRedirectId()
    {
        // Arrange
        int redirectId = 42;

        // Act
        var command = new DeleteRedirectCommand(redirectId);

        // Assert
        Assert.Equal(redirectId, command.RedirectId);
    }
}
