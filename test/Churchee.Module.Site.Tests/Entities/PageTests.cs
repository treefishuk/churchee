using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Events;

namespace Churchee.Module.Site.Tests.Entities
{
    public class PageTests
    {
        private static Guid TenantId => Guid.Parse("58d0e346-4f24-4adc-9b33-2fa57c903eae");
        private static Guid PageTypeId => Guid.Parse("57ff5f02-f390-41a9-8569-86e377117b1b");
        private static Guid PageTypeContentId => Guid.Parse("b81f268c-af88-46e0-b0dc-9c565e347b58");
        private static Guid PageId => Guid.Parse("8ee8211d-cd83-42f4-ac23-acb8ae92c98c");

        private static Page CreatePage(bool triggerEvents = false)
        {
            return new Page(TenantId, "Title", "url", "meta", PageTypeId, null, triggerEvents);
        }

        [Fact]
        public void Constructor_SetsProperties_AndOptionallyAddsDomainEvent()
        {
            // Act
            var page = CreatePage(triggerEvents: true);

            // Assert
            Assert.Equal(TenantId, page.ApplicationTenantId);
            Assert.Equal("Title", page.Title);
            Assert.Equal("url", page.Url);
            Assert.Equal("meta", page.Description);
            Assert.Equal(PageTypeId, page.PageTypeId);
            Assert.Equal(0, page.Version);
            Assert.NotNull(page.PageContent);
            Assert.Empty(page.PageContent);
            Assert.NotNull(page.DomainEvents);
            Assert.Contains(page.DomainEvents, e => e is PageCreatedEvent);
        }

        [Fact]
        public void AddContent_AddsPageContent()
        {
            // Arrange
            var page = CreatePage();
            var contentId = PageTypeContentId;
            var contentValue = "Some content";
            var version = 1;

            // Act
            page.AddContent(contentId, PageId, contentValue, version);

            // Assert
            Assert.Single(page.PageContent);
            var content = page.PageContent.First();
            Assert.Equal(contentId, content.PageTypeContentId);
            Assert.Equal(PageId, content.PageId);
            Assert.Equal(contentValue, content.Value);
            Assert.Equal(version, content.Version);
        }

        [Fact]
        public void UpdateInfo_UpdatesProperties_AndAddsDomainEvent()
        {
            // Arrange
            var page = CreatePage();
            var newTitle = "New Title";
            var newDesc = "New Desc";
            var newParentId = Guid.NewGuid();
            var newOrder = 5;

            // Act
            page.UpdateInfo(newTitle, newDesc, newParentId, newOrder);

            // Assert
            Assert.Equal(newTitle, page.Title);
            Assert.Equal(newDesc, page.Description);
            Assert.Equal(newParentId, page.ParentId);
            Assert.Equal(newOrder, page.Order);
            Assert.Contains(page.DomainEvents, e => e is PageInfoUpdatedEvent);
        }

        [Fact]
        public void UpdateContent_UpdatesExistingPageContentValues()
        {
            // Arrange
            var page = CreatePage();
            var contentId = PageTypeContentId;
            page.AddContent(contentId, PageId, "Old Value", 1);
            var newValue = "New Value";
            var updates = new List<KeyValuePair<Guid, string>>
            {
                new KeyValuePair<Guid, string>(contentId, newValue)
            };

            // Act
            page.UpdateContent(updates);

            // Assert
            Assert.Single(page.PageContent);
            Assert.Equal(newValue, page.PageContent.First().Value);
        }

        [Fact]
        public void Publish_IncrementsContentVersion_SerializesPublishedData_AndSetsPublished()
        {
            // Arrange
            var page = CreatePage();
            var contentId = PageTypeContentId;
            var devName = "devName";
            var value = "Content Value";
            var version = 1;

            // Add PageContent with PageTypeContent.DevName
            var pageContent = new PageContent(contentId, PageId, value, version)
            {
                PageTypeContent = new PageTypeContent(contentId, TenantId, "type", true, devName, 0)
            };
            page.PageContent.Add(pageContent);

            // Act
            page.Publish();

            // Assert
            Assert.True(page.Published);
            Assert.True(page.LastPublishedDate.HasValue);
            Assert.Equal(version + 1, page.PageContent.First().Version);
            Assert.False(string.IsNullOrEmpty(page.PublishedData));

            // Check that PublishedData contains the devName and value
            var json = page.PublishedData;
            Assert.Contains(devName, json);
            Assert.Contains(value, json);
        }

        [Fact]
        public void Publish_DoesNothing_WhenNoContent()
        {
            // Arrange
            var page = CreatePage();

            // Act
            page.Publish();

            // Assert
            Assert.True(page.Published);
            Assert.True(page.LastPublishedDate.HasValue);
            Assert.NotNull(page.PublishedData);
        }

        [Fact]
        public void Unpublish_SetsPublishedFalse()
        {
            // Arrange
            var page = CreatePage();
            page.Publish();

            // Act
            page.Unpublish();

            // Assert
            Assert.False(page.Published);
        }
    }
}