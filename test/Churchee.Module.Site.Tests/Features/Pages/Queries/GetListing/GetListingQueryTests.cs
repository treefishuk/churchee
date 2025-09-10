using Churchee.Module.Site.Features.Pages.Queries;

public class GetListingQueryTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        Guid? parentId = Guid.NewGuid();
        string searchText = "search";

        // Act
        var query = new GetListingQuery(parentId, searchText);

        // Assert
        Assert.Equal(parentId, query.ParentId);
        Assert.Equal(searchText, query.SearchText);
    }
}
