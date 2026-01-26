
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Registration;
using Churchee.Test.Helpers.Validation;
using Microsoft.EntityFrameworkCore;

namespace Churchee.Module.Site.Tests.Registrations
{
    public class EntityRegistrationsTests
    {
        [Fact]
        public void EntityRegistrationsTests_WebContent_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(WebContent));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("WebContent");

            // Assert Properties
            eventEntityType?.FindProperty(nameof(WebContent.Order))?.GetDefaultValue().Should().Be(10);
            eventEntityType?.FindProperty(nameof(WebContent.PublishedData))?.GetColumnType()?.Should().Be("nvarchar(max)");
        }

        [Fact]
        public void EntityRegistrationsTests_Page_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Page));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("Pages");
        }

        [Fact]
        public void EntityRegistrationsTests_MediaItem_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(MediaItem));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("MediaItems");

            // Assert Properties
            eventEntityType?.FindProperty(nameof(MediaItem.Html))?.GetMaxLength().Should().Be(1000);
        }

        [Fact]
        public void EntityRegistrationsTests_MediaFolder_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(MediaFolder));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("MediaFolders");
        }

        [Fact]
        public void EntityRegistrationsTests_PageType_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageType));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageType");
        }

        [Fact]
        public void EntityRegistrationsTests_PageTypeProperty_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageTypeProperty));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageTypeProperty");
        }

        [Fact]
        public void EntityRegistrationsTests_PageTypeTypeMapping_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageTypeTypeMapping));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageTypeTypeMapping");
        }

        [Fact]
        public void EntityRegistrationsTests_PageTypeContent_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageTypeContent));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageTypeContent");
        }

        [Fact]
        public void EntityRegistrationsTests_ViewTemplate_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(ViewTemplate));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("ViewTemplates");

            eventEntityType?.IsTemporal().Should().BeTrue();
        }

        [Fact]
        public void EntityRegistrationsTests_RedirectUrl_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(RedirectUrl));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("RedirectUrl");
        }

        [Fact]
        public void EntityRegistrationsTests_Css_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Css));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("CSS");
            eventEntityType?.IsTemporal().Should().BeTrue();
        }

        [Fact]
        public void EntityRegistrationsTests_Article_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(Article));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("Articles");
        }

        [Fact]
        public void EntityRegistrationsTests_PageContent_ConfiguredCorrectly()
        {
            // Arrange
            var modelBuilder = GetBuilder();

            // Act
            var eventEntityType = modelBuilder.Model.FindEntityType(typeof(PageTypeContent));

            // Assert Table Name
            eventEntityType?.GetTableName()?.Should().Be("PageTypeContent");
        }


        private static ModelBuilder GetBuilder()
        {
            // Arrange
            var modelBuilder = new ModelBuilder(new Microsoft.EntityFrameworkCore.Metadata.Conventions.ConventionSet());
            var cut = new EntityRegistrations();

            // Act
            cut.RegisterEntities(modelBuilder);
            return modelBuilder;
        }
    }
}
