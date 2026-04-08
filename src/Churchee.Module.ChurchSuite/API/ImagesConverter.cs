using System.Text.Json;
using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class ImagesConverter : JsonConverter<Images>
    {
        public override Images Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            // Case B: images: []
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return new Images();
            }

            // Case A: images: { ... }
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                var images = new Images();

                using var doc = JsonDocument.ParseValue(ref reader);

                var root = doc.RootElement;

                if (root.TryGetProperty("thumb", out var thumbObj))
                {
                    images.Thumb = thumbObj.Deserialize<Image>(options);
                }

                if (root.TryGetProperty("sm", out var smObj))
                {
                    images.Small = smObj.Deserialize<Image>(options);
                }

                if (root.TryGetProperty("md", out var mdObj))
                {
                    images.Medium = mdObj.Deserialize<Image>(options);
                }

                if (root.TryGetProperty("lg", out var lgObj))
                {
                    images.Large = lgObj.Deserialize<Image>(options);
                }

                return images;
            }

            throw new JsonException("Unexpected JSON for Images");
        }

        public override void Write(Utf8JsonWriter writer, Images value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("thumb");
            JsonSerializer.Serialize(writer, value.Thumb, options);

            writer.WritePropertyName("sm");
            JsonSerializer.Serialize(writer, value.Small, options);

            writer.WritePropertyName("md");
            JsonSerializer.Serialize(writer, value.Medium, options);

            writer.WritePropertyName("lg");
            JsonSerializer.Serialize(writer, value.Large, options);

            writer.WriteEndObject();
        }
    }


}
