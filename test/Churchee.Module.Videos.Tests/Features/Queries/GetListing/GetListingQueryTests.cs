using Churchee.Module.Videos.Features.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Tests.Features.Queries.GetListing
{
    public class GetListingQueryTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            int skip = 5;
            int take = 10;
            string searchText = "test";
            string orderBy = "Title";

            // Act
            var query = new GetListingQuery(skip, take, searchText, orderBy);

            // Assert
            query.Skip.Should().Be(skip);
            query.Take.Should().Be(take);
            query.SearchText.Should().Be(searchText);
            query.OrderBy.Should().Be(orderBy);
        }

        [Fact]
        public void Constructor_DoesNotAllowsNegativeTake()
        {
            // Arrange
            int skip = 0;
            int take = -10;
            string searchText = "";
            string orderBy = "";

            // Act
            Action act = () => { var _ = new GetListingQuery(skip, take, searchText, orderBy); };

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("Take must be zero or positive. (Parameter 'take')");
        }

        [Fact]
        public void Constructor_DoesNotAllowsNegativeSkip()
        {
            // Arrange
            int skip = -1;
            int take = 10;
            string searchText = "";
            string orderBy = "";


            // Act
            Action act = () => { var _ = new GetListingQuery(skip, take, searchText, orderBy); };

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>("Skip must be zero or positive. (Parameter 'skip')");
        }
    }
}