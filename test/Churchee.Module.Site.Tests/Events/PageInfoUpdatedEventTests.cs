using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Events
{
    public class PageInfoUpdatedEventTests
    {
        [Fact]
        public void Constructor_SetsPageIdCorrectly()
        {
            // Arrange
            var pageId = Guid.NewGuid();

            // Act
            var evt = new PageInfoUpdatedEvent(pageId);

            // Assert
            Assert.Equal(pageId, evt.PageId);
        }
    }
}