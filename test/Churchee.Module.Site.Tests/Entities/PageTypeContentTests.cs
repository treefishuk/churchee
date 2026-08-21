using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class PageTypeContentTests
    {
        private readonly Guid _ContentId;
        private readonly Guid _TenantId;

        public PageTypeContentTests()
        {
            _ContentId = Guid.NewGuid();
            _TenantId = Guid.NewGuid();
        }

        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var type = "text";
            var isRequired = true;
            var name = "Content Name";
            var order = 2;

            // Act
            var content = new PageTypeContent(_ContentId, _TenantId, type, isRequired, name, order, null, "Help Text");

            // Assert
            Assert.Equal(_ContentId, content.Id);
            Assert.Equal(_TenantId, content.ApplicationTenantId);
            Assert.Equal(type, content.Type);
            Assert.Equal(isRequired, content.IsRequired);
            Assert.Equal(name, content.Name);
            Assert.Equal(order, content.Order);
            Assert.Equal("Help Text", content.HelpText);
            Assert.False(string.IsNullOrWhiteSpace(content.DevName));
        }

        [Fact]
        public void UpdateDetails_UpdatesAllFields()
        {
            // Arrange
            var content = new PageTypeContent(_ContentId, _TenantId, "text", false, "Old Name", 1, null, "Old Help Text");
            var newIsRequired = true;
            var newName = "New Name";
            var newType = "html";
            var newOrder = 5;

            // Act
            content.UpdateDetails(newIsRequired, newName, newType, newOrder, null, "New Help Text");

            // Assert
            Assert.Equal(newIsRequired, content.IsRequired);
            Assert.Equal(newName, content.Name);
            Assert.Equal(newType, content.Type);
            Assert.Equal(newOrder, content.Order);
            Assert.Equal("New Help Text", content.HelpText);
        }
    }
}