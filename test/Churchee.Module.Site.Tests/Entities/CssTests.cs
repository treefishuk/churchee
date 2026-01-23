using Churchee.Module.Site.Entities;

namespace Churchee.Module.Site.Tests.Entities
{
    public class CssTests
    {
        [Fact]
        public void Constructor_SetsApplicationTenantId()
        {
            // Arrange
            var tenantId = Guid.NewGuid();

            // Act
            var css = new Css(tenantId);

            // Assert
            Assert.Equal(tenantId, css.ApplicationTenantId);
        }

        [Fact]
        public void SetStyles_SetsStylesProperty()
        {
            // Arrange
            var css = new Css(Guid.NewGuid());
            var styles = "body { color: red; }";

            // Act
            css.SetStyles(styles);

            // Assert
            Assert.Equal(styles, css.Styles);
        }

        [Fact]
        public void SetMinifiedStyles_SetsMinifiedStylesProperty()
        {
            // Arrange
            var css = new Css(Guid.NewGuid());
            var minified = "body{color:red;}";

            // Act
            css.SetMinifiedStyles(minified);

            // Assert
            Assert.Equal(minified, css.MinifiedStyles);
        }
    }
}