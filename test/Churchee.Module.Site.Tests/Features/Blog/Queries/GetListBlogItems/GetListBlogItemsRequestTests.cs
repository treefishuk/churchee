using Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems;
using Xunit;

public class GetListBlogItemsRequestTests
{
    [Fact]
    public void Constructor_SetsPropertiesCorrectly()
    {
        // Arrange
        int skip = 1;
        int take = 10;
        string searchText = "search";
        string orderBy = "Title";

        // Act
        var request = (GetListBlogItemsRequest)System.Activator.CreateInstance(
            typeof(GetListBlogItemsRequest),
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
            null,
            new object[] { skip, take, searchText, orderBy },
            null);

        // Assert
        Assert.Equal(skip, request.Skip);
        Assert.Equal(take, request.Take);
        Assert.Equal(searchText, request.SearchText);
        Assert.Equal(orderBy, request.OrderBy);
    }
}
