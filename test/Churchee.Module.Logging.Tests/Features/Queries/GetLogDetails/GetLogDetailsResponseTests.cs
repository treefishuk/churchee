using Churchee.Module.Logging.Features.Queries;

namespace Churchee.Module.Logging.Tests.Features.Queries.GetLogDetails
{
    public class GetLogDetailsResponseTests
    {
        [Fact]
        public void Properties_ParsesValidXmlString()
        {
            // Arrange
            string xml = @"<properties>
                            <property key=""UserId"">42</property>
                            <property key=""Action"">Login</property>
                        </properties>";
            var response = new GetLogDetailsResponse
            {
                PropertiesString = xml
            };

            // Act
            var props = response.Properties;

            // Assert
            Assert.NotNull(props);
            Assert.Equal(2, props.Count);
            Assert.Equal("UserId", props[0].Key);
            Assert.Equal("42", props[0].Value);
            Assert.Equal("Action", props[1].Key);
            Assert.Equal("Login", props[1].Value);
        }

        [Fact]
        public void Properties_ThrowsOnEmptyXmlString()
        {
            // Arrange
            var response = new GetLogDetailsResponse
            {
                PropertiesString = ""
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _ = response.Properties);
        }

        [Fact]
        public void Properties_ThrowsOnMalformedXml()
        {
            // Arrange
            var response = new GetLogDetailsResponse
            {
                PropertiesString = "<properties><property key=\"UserId\">42</property" // missing closing tag
            };

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => _ = response.Properties);
        }

        [Fact]
        public void Properties_ParsesSingleProperty()
        {
            // Arrange
            string xml = @"<properties>
                            <property key=""TestKey"">TestValue</property>
                        </properties>";
            var response = new GetLogDetailsResponse
            {
                PropertiesString = xml
            };

            // Act
            var props = response.Properties;

            // Assert
            Assert.Single(props);
            Assert.Equal("TestKey", props[0].Key);
            Assert.Equal("TestValue", props[0].Value);
        }
    }
}
