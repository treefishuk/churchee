using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class ArticleTests
    {
        private static Article CreateArticle(
            Guid? tenantId = null,
            Guid? pageTypeId = null,
            Guid? parentId = null,
            string title = "Test Title",
            string url = "test-url",
            string description = "Test Description")
        {
            return new Article(
                tenantId ?? Guid.NewGuid(),
                pageTypeId ?? Guid.NewGuid(),
                parentId ?? Guid.NewGuid(),
                title,
                url,
                description
            );
        }

        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var title = "Title";
            var url = "url";
            var description = "desc";

            // Act
            var article = new Article(tenantId, pageTypeId, parentId, title, url, description);

            // Assert
            Assert.Equal(title, article.Title);
            Assert.Equal(url, article.Url);
            Assert.Equal(description, article.Description);
            Assert.Equal(pageTypeId, article.PageTypeId);
            Assert.Equal(parentId, article.ParentId);
            Assert.True(article.IsSystem);
        }

        [Fact]
        public void SetContent_SetsContentProperty()
        {
            // Arrange
            var article = CreateArticle();
            var content = "<p>Some content</p>";

            // Act
            article.SetContent(content);

            // Assert
            Assert.Equal(content, article.Content);
        }

        [Fact]
        public void SetImage_SetsImageUrlAndAltTag()
        {
            // Arrange
            var article = CreateArticle();
            var url = "http://image.url";
            var alt = "alt text";

            // Act
            article.SetImage(url, alt);

            // Assert
            Assert.Equal(url, article.ImageUrl);
            Assert.Equal(alt, article.ImageAltTag);
        }

        [Fact]
        public void SetPublishDate_SetsLastPublishedDate()
        {
            // Arrange
            var article = CreateArticle();
            var date = new DateTime(2024, 1, 1);

            // Act
            article.SetPublishDate(date);

            // Assert
            Assert.Equal(date, article.LastPublishedDate);
        }

        [Fact]
        public void Publish_SetsPublishedTrueAndUpdatesLastPublishedDate()
        {
            // Arrange
            var article = CreateArticle();

            // Act
            article.Publish();

            // Assert
            Assert.True(article.Published);
            Assert.True(article.LastPublishedDate.HasValue);
            Assert.True((DateTime.Now - article.LastPublishedDate.Value).TotalSeconds < 5);
        }

        [Fact]
        public void UnPublish_SetsPublishedFalse()
        {
            // Arrange
            var article = CreateArticle();
            article.Publish();

            // Act
            article.UnPublish();

            // Assert
            Assert.False(article.Published);
        }

        [Fact]
        public void UpdateInfo_UpdatesTitleDescriptionParentId_AndAddsDomainEvent()
        {
            // Arrange
            var article = CreateArticle();
            var newTitle = "New Title";
            var newDescription = "New Description";
            var newParentId = Guid.NewGuid();

            // Act
            article.UpdateInfo(newTitle, newDescription, newParentId);

            // Assert
            Assert.Equal(newTitle, article.Title);
            Assert.Equal(newDescription, article.Description);
            Assert.Equal(newParentId, article.ParentId);
            Assert.NotEmpty(article.DomainEvents);
        }
    }
}