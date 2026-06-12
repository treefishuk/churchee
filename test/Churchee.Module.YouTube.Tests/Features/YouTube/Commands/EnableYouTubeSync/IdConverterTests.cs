using Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync;
using Churchee.Test.Helpers.Validation;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.EnableYouTubeSync
{
    public class IdConverterTests
    {
        [Fact]
        public void Can_Convert_Id_Object_To_String()
        {
            // Arrange
            string json = """"
            {
                "id": {
                    "videoId": "abc123"
                }
            }

            """";

            // Act
            var result = JsonSerializer.Deserialize<TestClass>(json);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("abc123");
        }

        [Fact]
        public void Returns_Straight_Id_When_String()
        {
            // Arrange
            string json = """"
            {
                "id":"abc123"
            }

            """";

            // Act
            var result = JsonSerializer.Deserialize<TestClass>(json);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be("abc123");
        }

        private class TestClass
        {
            [JsonPropertyName("id")]
            [JsonConverter(typeof(IdConverter))]
            public string Id { get; set; }
        }
    }



}
