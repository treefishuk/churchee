using Churchee.Module.Site.Features.Pages.Queries;

namespace Churchee.Module.Site.Tests.Features.Pages.Queries.GetListing
{
    public class GetListingQueryResponseItemTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var item = new GetListingQueryResponseItem
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Url = "/test-url",
                Modified = DateTime.UtcNow,
                HasChildren = true,
                ParentId = Guid.NewGuid(),
                Published = true
            };

            // Assert
            Assert.Equal("Test Title", item.Title);
            Assert.Equal("/test-url", item.Url);
            Assert.NotNull(item.Modified);
            Assert.True(item.HasChildren);
            Assert.NotNull(item.ParentId);
            Assert.True(item.Published);
        }
    }
}
