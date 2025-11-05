using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Site.Tests.Entities
{
    public class PageTypeTests
    {
        public PageTypeTests()
        {
            _Id = Guid.NewGuid();
            _SystemKey = Guid.NewGuid();
            _TenantId = Guid.NewGuid();
        }

        private readonly Guid _Id;
        private readonly Guid _SystemKey;
        private readonly Guid _TenantId;

        [Fact]
        public void Constructor_SetsProperties_AndOptionallyAddsDomainEvent()
        {
            // Act
            var pageType = new PageType(_Id, _SystemKey, _TenantId, true, "My Page Type", triggerPageTypeCreatedEvent: true);

            // Assert
            Assert.Equal(_Id, pageType.Id);
            Assert.Equal(_SystemKey, pageType.SystemKey);
            Assert.Equal(_TenantId, pageType.ApplicationTenantId);
            Assert.True(pageType.AllowInRoot);
            Assert.Equal("My Page Type", pageType.Name);
            Assert.Equal("MyPageType", pageType.DevName);
            Assert.NotNull(pageType.ParentTypes);
            Assert.NotNull(pageType.ChildrenTypes);
            Assert.NotNull(pageType.PageTypeProperties);
            Assert.NotNull(pageType.PageTypeContent);
            Assert.NotNull(pageType.Pages);
            Assert.NotNull(pageType.DomainEvents);
            Assert.Contains(pageType.DomainEvents, e => e is PageTypeCreatedEvent);
        }

        [Fact]
        public void Constructor_DoesNotAddDomainEvent_WhenFlagIsFalse()
        {
            // Act
            var pageType = new PageType(_Id, _SystemKey, _TenantId, false, "No Event", triggerPageTypeCreatedEvent: false);

            // Assert
            pageType.DomainEvents.Should().BeNull();
        }

        [Fact]
        public void AddPageTypeContent_AddsContent_AndAddsDomainEvent()
        {
            // Arrange
            var pageType = new PageType(_Id, _SystemKey, _TenantId, true, "Type");
            var contentId = Guid.NewGuid();
            var name = "Content Name";
            var type = "text";
            var required = true;
            var order = 1;

            // Act
            pageType.AddPageTypeContent(contentId, name, type, required, order);

            // Assert
            Assert.Single(pageType.PageTypeContent);
            var content = pageType.PageTypeContent.First();
            Assert.Equal(contentId, content.Id);
            Assert.Equal(name, content.Name);
            Assert.Equal(type, content.Type);
            Assert.Equal(required, content.IsRequired);
            Assert.Equal(order, content.Order);
            Assert.Contains(pageType.DomainEvents, e => e is PageTypeContentCreatedEvent);
        }

        [Fact]
        public void AddChildType_AddsChildTypeMapping()
        {
            // Arrange
            var parent = new PageType(_Id, _SystemKey, _TenantId, true, "Parent");
            var child = new PageType(Guid.NewGuid(), Guid.NewGuid(), _TenantId, false, "Child");

            // Act
            parent.AddChildType(child);

            // Assert
            Assert.Single(parent.ChildrenTypes);
            var mapping = parent.ChildrenTypes.First();
            Assert.Equal(parent, mapping.ParentPageType);
            Assert.Equal(child, mapping.ChildPageType);
        }
    }
}