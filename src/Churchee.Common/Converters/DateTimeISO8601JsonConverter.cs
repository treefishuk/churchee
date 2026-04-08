using System;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Churchee.Common.Converters
{
    public class DateTimeIso8601JsonConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));

            string s = reader.GetString();

            if (string.IsNullOrWhiteSpace(s))
            {
                return default;
            }

            // Try round-trip ISO 8601 first, then fall back to Parse
            if (DateTime.TryParseExact(s, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var dt))
            {
                return dt;
            }

            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dt))
            {
                return dt;
            }

            // Let ParseExact throw if you want a hard failure; otherwise throw a clear FormatException
            throw new FormatException($"Invalid DateTime format: '{s}'");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString());
        }
    }
}
