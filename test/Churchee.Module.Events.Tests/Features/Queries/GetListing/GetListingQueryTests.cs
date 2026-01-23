using Churchee.Module.Events.Features.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Events.Tests.Features.Queries.GetListing
{
    public class GetListingQueryTests
    {
        [Fact]
        public void GetListingQuery_ParametersSet()
        {
            //arrange
            int skip = 2;
            int take = 3;
            string searchText = "Find a thing";
            string orderBy = "CreatedDate desc";

            //act
            var cut = new GetListingQuery(skip, take, searchText, orderBy);

            //assert
            cut.Skip.Should().Be(skip);
            cut.Take.Should().Be(take);
            cut.SearchText.Should().Be(searchText);
            cut.OrderBy.Should().Be("CreatedDate");
            cut.OrderByDirection.Should().Be("desc");
        }
    }
}
