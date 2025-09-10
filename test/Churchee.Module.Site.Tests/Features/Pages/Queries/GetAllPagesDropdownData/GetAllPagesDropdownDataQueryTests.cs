using Churchee.Module.Site.Features.Pages.Queries;

namespace Churchee.Module.Site.Tests.Features.Pages.Queries.GetAllPagesDropdownData
{
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
}
