using Churchee.Module.Site.Features.PageTypes.Queries.GetPageOfPageTypeContent;

public class GetContentTypesForPageTypeQueryTests
{
    [Fact]
    public void Constructor_SetsPageTypeId()
    {
        // Arrange
        var pageTypeId = Guid.NewGuid();

        // Act
        var query = new GetContentTypesForPageTypeQuery(pageTypeId);

        // Assert
        Assert.Equal(pageTypeId, query.PageTypeId);
    }
}
