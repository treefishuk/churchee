using Churchee.Module.Facebook.Events.Helpers;
using System.Globalization;
using System.Text.Json;

namespace Churchee.Module.Facebook.Events.Tests.Helpers
{
    public class DateTimeIso8601JsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public DateTimeIso8601JsonConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new DateTimeIso8601JsonConverter() }
            };
        }

        [Fact]
        public void Read_ParsesIso8601String_AsDateTime()
        {
            // Arrange
            var json = "\"2024-05-31T14:30:00\"";
            var expected = DateTime.Parse("2024-05-31T14:30:00", new CultureInfo("en-GB"));

            // Act
            var result = JsonSerializer.Deserialize<DateTime>(json, _options);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Write_SerializesDateTime_AsString()
        {
            // Arrange
            var date = new DateTime(2024, 5, 31, 14, 30, 0);
            var expected = $"\"{date}\"";

            // Act
            var json = JsonSerializer.Serialize(date, _options);

            // Assert
            Assert.Equal(expected, json);
        }

        [Fact]
        public void Read_ThrowsFormatException_OnInvalidDate()
        {
            // Arrange
            var json = "\"not-a-date\"";

            // Act & Assert
            Assert.Throws<FormatException>(() =>
                JsonSerializer.Deserialize<DateTime>(json, _options));
        }
    }
}
