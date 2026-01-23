using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Events
{
    public class PageCreatedEventTests
    {
        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            // Arrange
            var pageId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();

            // Act
            var evt = new PageCreatedEvent(pageId, pageTypeId);

            // Assert
            Assert.Equal(pageId, evt.PageId);
            Assert.Equal(pageTypeId, evt.PageTypeId);
        }
    }
}