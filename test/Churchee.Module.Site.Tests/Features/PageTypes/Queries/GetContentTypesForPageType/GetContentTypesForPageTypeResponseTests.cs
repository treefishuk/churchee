using Churchee.Module.Site.Features.PageTypes.Queries;

public class GetContentTypesForPageTypeResponseTests
{
    [Fact]
    public void CanSetAndGetProperties()
    {
        // Arrange

        var id = Guid.NewGuid();

        var response = new GetContentTypesForPageTypeResponse
        {
            Id = id,
            Name = "Name",
            DevName = "DevName",
            Type = "string",
            Required = true,
            Order = 1
        };

        // Assert
        Assert.Equal(id, response.Id);
        Assert.Equal("Name", response.Name);
        Assert.Equal("DevName", response.DevName);
        Assert.Equal("string", response.Type);
        Assert.True(response.Required);
        Assert.Equal(1, response.Order);
    }
}
