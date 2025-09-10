using Churchee.Module.Site.Features.Pages.Queries;

public class GetAllPagesDropdownDataQueryTests
{
    [Fact]
    public void CanInstantiateQuery()
    {
        // Act
        var query = new GetAllPagesDropdownDataQuery();
        // Assert
        Assert.NotNull(query);
    }
}
