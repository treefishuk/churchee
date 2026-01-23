using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class PageContentTests
    {
        [Fact]
        public void DefaultConstructor_InitializesVersionToZero()
        {
            // Act
            var content = new PageContent();

            // Assert
            Assert.Equal(0, content.Version);
        }

        [Fact]
        public void ParameterizedConstructor_SetsProperties()
        {
            // Arrange
            var pageTypeContentId = Guid.NewGuid();
            var pageId = Guid.NewGuid();
            var value = "Test Value";
            var version = 3;

            // Act
            var content = new PageContent(pageTypeContentId, pageId, value, version);

            // Assert
            Assert.Equal(pageTypeContentId, content.PageTypeContentId);
            Assert.Equal(pageId, content.PageId);
            Assert.Equal(value, content.Value);
            Assert.Equal(version, content.Version);
        }

        [Fact]
        public void IncrementVersion_IncreasesVersionByOne()
        {
            // Arrange
            var content = new PageContent(Guid.NewGuid(), Guid.NewGuid(), "val", 2);

            // Act
            content.IncrementVersion();

            // Assert
            Assert.Equal(3, content.Version);
        }

        [Fact]
        public void Can_Set_And_Get_Deleted()
        {
            // Arrange & Act
            var content = new PageContent
            {
                Deleted = true
            };

            // Assert
            Assert.True(content.Deleted);
        }

        [Fact]
        public void Can_Set_And_Get_NavigationProperties()
        {
            // Arrange
            var content = new PageContent();
            var page = new Page(Guid.NewGuid(), "t", "u", "m", Guid.NewGuid(), null, false);
            var pageTypeContent = new PageTypeContent(Guid.NewGuid(), Guid.NewGuid(), "type", true, "dev", 1);

            // Act
            content.Page = page;
            content.PageTypeContent = pageTypeContent;

            // Assert
            Assert.Equal(page, content.Page);
            Assert.Equal(pageTypeContent, content.PageTypeContent);
        }
    }
}