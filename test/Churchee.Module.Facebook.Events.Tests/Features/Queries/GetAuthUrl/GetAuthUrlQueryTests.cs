using Churchee.Module.Facebook.Events.Features.Queries;

namespace Churchee.Module.Facebook.Events.Tests.Features.Queries
{
    public class GetAuthUrlQueryTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var domain = "https://example.com";
            var pageId = "123456";

            // Act
            var query = new GetAuthUrlQuery(domain, pageId);

            // Assert
            Assert.Equal(domain, query.Domain);
            Assert.Equal(pageId, query.PageId);
        }

        [Fact]
        public void Properties_CanBeSetAndGet()
        {
            // Arrange
            var query = new GetAuthUrlQuery("https://foo.com", "init");

            // Act
            query.Domain = "https://bar.com";
            query.PageId = "changedId";

            // Assert
            Assert.Equal("https://bar.com", query.Domain);
            Assert.Equal("changedId", query.PageId);
        }
    }
}
