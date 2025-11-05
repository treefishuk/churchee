using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class MediaItemTests
    {
        private static Guid TenantId => Guid.Parse("58d0e346-4f24-4adc-9b33-2fa57c903eae");
        private static Guid FolderId => Guid.Parse("57ff5f02-f390-41a9-8569-86e377117b1b");

        [Fact]
        public void Constructor_SetsProperties_Basic()
        {
            // Arrange
            var title = "Test Title";
            var url = "http://media.url";
            var description = "Test Description";
            Guid? mediaFolderId = FolderId;

            // Act
            var item = new MediaItem(TenantId, title, url, description, mediaFolderId);

            // Assert
            Assert.Equal(TenantId, item.ApplicationTenantId);
            Assert.Equal(title, item.Title);
            Assert.Equal(url, item.MediaUrl);
            Assert.Equal(description, item.Description);
            Assert.Equal(mediaFolderId, item.MediaFolderId);
            Assert.Null(item.Html);
            Assert.Null(item.LinkUrl);
            Assert.Null(item.CssClass);
            Assert.Null(item.Order);
        }

        [Fact]
        public void Constructor_SetsProperties_Extended()
        {
            // Arrange
            var title = "Test Title";
            var url = "http://media.url";
            var description = "Test Description";
            var html = "<p>HTML</p>";
            Guid? mediaFolderId = FolderId;
            var linkUrl = "http://link.url";
            var cssClass = "media-class";

            // Act
            var item = new MediaItem(TenantId, title, url, description, html, mediaFolderId, linkUrl, cssClass);

            // Assert
            Assert.Equal(TenantId, item.ApplicationTenantId);
            Assert.Equal(title, item.Title);
            Assert.Equal(url, item.MediaUrl);
            Assert.Equal(description, item.Description);
            Assert.Equal(html, item.Html);
            Assert.Equal(mediaFolderId, item.MediaFolderId);
            Assert.Equal(linkUrl, item.LinkUrl);
            Assert.Equal(cssClass, item.CssClass);
            Assert.Equal(10, item.Order);
        }

        [Fact]
        public void UpdateMediaUrl_Updates_WhenNotNullOrEmpty()
        {
            // Arrange
            var item = new MediaItem(TenantId, "Title", "oldUrl", "desc");
            var newUrl = "newUrl";

            // Act
            item.UpdateMediaUrl(newUrl);

            // Assert
            Assert.Equal(newUrl, item.MediaUrl);
        }

        [Fact]
        public void UpdateMediaUrl_DoesNotUpdate_WhenNullOrEmpty()
        {
            // Arrange
            var originalUrl = "originalUrl";
            var item = new MediaItem(TenantId, "Title", originalUrl, "desc");

            // Act
            item.UpdateMediaUrl(null);
            Assert.Equal(originalUrl, item.MediaUrl);

            item.UpdateMediaUrl("");
            Assert.Equal(originalUrl, item.MediaUrl);
        }

        [Fact]
        public void UpdateDetails_UpdatesAllFields()
        {
            // Arrange
            var item = new MediaItem(TenantId, "Title", "url", "desc", "html", FolderId, "link", "css");
            var newTitle = "New Title";
            var newDesc = "New Desc";
            var newHtml = "<div>New</div>";
            var newLink = "http://new.link";
            var newCss = "new-css";
            var newOrder = 42;

            // Act
            item.UpdateDetails(newTitle, newDesc, newHtml, newLink, newCss, newOrder);

            // Assert
            Assert.Equal(newTitle, item.Title);
            Assert.Equal(newDesc, item.Description);
            Assert.Equal(newHtml, item.Html);
            Assert.Equal(newLink, item.LinkUrl);
            Assert.Equal(newCss, item.CssClass);
            Assert.Equal(newOrder, item.Order);
        }

        [Fact]
        public void Can_Set_And_Get_MediaFolder()
        {
            // Arrange
            var item = new MediaItem(TenantId, "Title", "url", "desc");
            var folder = new MediaFolder(TenantId, "Folder", ".jpg");

            // Act
            item.MediaFolder = folder;

            // Assert
            Assert.Equal(folder, item.MediaFolder);
        }
    }
}