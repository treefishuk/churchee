using Churchee.Module.Site.Features.Blog.Responses;

public class GetListBlogItemsResponseItemTests
{
    [Fact]
    public void CanSetAndGetProperties()
    {
        // Arrange
        var item = new GetListBlogItemsResponseItem
        {
            Id = Guid.NewGuid(),
            Title = "Test Title",
            Url = "/test-url",
            Published = true,
            Modified = DateTime.UtcNow
        };

        // Assert
        Assert.Equal("Test Title", item.Title);
        Assert.Equal("/test-url", item.Url);
        Assert.True(item.Published);
        Assert.NotNull(item.Modified);
    }
}
