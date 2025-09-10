using Churchee.Module.Site.Features.Pages.Queries;

namespace Churchee.Module.Site.Tests.Features.Pages.Queries.GetListing
{
    public class GetListingQueryTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            Guid? parentId = Guid.NewGuid();
            string searchText = "search";

            // Act
            var query = new GetListingQuery(1, 1, searchText, "Url desc", parentId);

            // Assert
            Assert.Equal(parentId, query.ParentId);
            Assert.Equal(searchText, query.SearchText);
            Assert.Equal(1, query.Take);
            Assert.Equal(1, query.Skip);
            Assert.Equal("Url", query.OrderBy);
            Assert.Equal("desc", query.OrderByDirection);
        }
    }
}
