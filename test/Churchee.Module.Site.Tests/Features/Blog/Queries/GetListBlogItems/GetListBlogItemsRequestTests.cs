using Churchee.Module.Site.Features.Blog.Queries.GetListBlogItems;

namespace Churchee.Module.Site.Tests.Features.Blog.Queries.GetListBlogItems
{
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
            var request = new GetListBlogItemsRequest(skip, take, searchText, orderBy);

            // Assert
            Assert.Equal(skip, request.Skip);
            Assert.Equal(take, request.Take);
            Assert.Equal(searchText, request.SearchText);
            Assert.Equal(orderBy, request.OrderBy);
        }
    }
}
