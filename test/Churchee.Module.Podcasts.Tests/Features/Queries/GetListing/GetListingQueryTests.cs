using Churchee.Module.Podcasts.Features.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Podcasts.Tests.Features.Queries.GetListing
{
    public class GetListingQueryTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            int skip = 10;
            int take = 20;
            string searchText = "test";
            string orderBy = "title";

            // Act
            var query = new GetListingQuery(skip, take, searchText, orderBy);

            // Assert
            query.Skip.Should().Be(skip);
            query.Take.Should().Be(take);
            query.SearchText.Should().Be(searchText);
            query.OrderBy.Should().Be(orderBy);
        }
    }

}
