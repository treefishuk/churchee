using Churchee.Common.Converters;
using System.Globalization;
using System.Text.Json;

namespace Churchee.Common.Tests.Converters
{
    public class DateTimeIso8601JsonConverterTests
    {
        private JsonSerializerOptions Options()
        {
            var o = new JsonSerializerOptions();
            o.Converters.Add(new DateTimeIso8601JsonConverter());
            return o;
        }

        [Fact]
        public void Read_FallbackParse_AssumesUniversalAndAdjustsToUtc()
        {
            // Non-o format that should be parsed using the fallback path
            var input = "2021-05-01 12:34:56";

            string json = $"\"{input}\"";

            var expected = DateTime.Parse(input, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);

            var result = JsonSerializer.Deserialize<DateTime>(json, Options());

            Assert.Equal(expected, result);
            Assert.Equal(DateTimeKind.Utc, result.Kind);
        }

        [Fact]
        public void Read_EmptyString_ReturnsDefaultDateTime()
        {
            string json = "\"\"";

            var result = JsonSerializer.Deserialize<DateTime>(json, Options());

            Assert.Equal(default, result);
        }

        [Fact]
        public void Read_InvalidFormat_ThrowsFormatException()
        {
            string json = "\"not-a-date\"";

            Assert.Throws<FormatException>(() => JsonSerializer.Deserialize<DateTime>(json, Options()));
        }

        [Fact]
        public void Write_WritesStringToken()
        {
            var dt = new DateTime(2022, 1, 2, 3, 4, 5);
            string json = JsonSerializer.Serialize(dt, Options());

            // Ensure the converter produced a JSON string value (starts and ends with quotes)
            Assert.False(string.IsNullOrEmpty(json));
            Assert.Equal('\"', json[0]);
            Assert.Equal('\"', json[^1]);
        }
    }
}