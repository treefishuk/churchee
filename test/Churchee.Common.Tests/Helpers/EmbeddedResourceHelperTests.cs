using Churchee.Common.Helpers;

namespace Churchee.Common.Tests.Helpers
{
    public class EmbeddedResourceHelperTests
    {
        [Fact]
        public void ReadTemplate_WhenResourceExists_ReturnsTemplateContents()
        {
            string result = EmbeddedResourceHelper.ReadTemplate("TestTemplate");

            Assert.Equal("<h1>Embedded test template</h1>\n", result);
        }

        [Fact]
        public void ReadTemplate_WhenResourceDoesNotExist_ThrowsInvalidOperationException()
        {
            var exception = Assert.Throws<InvalidOperationException>(() => EmbeddedResourceHelper.ReadTemplate("MissingTemplate"));

            Assert.Contains(".Templates.MissingTemplate.cshtml", exception.Message);
        }
    }
}
