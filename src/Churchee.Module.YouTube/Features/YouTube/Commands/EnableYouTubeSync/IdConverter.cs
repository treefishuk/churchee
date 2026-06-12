using System.Text.Json;
using System.Text.Json.Serialization;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync
{
    public class IdConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString() ?? string.Empty;
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var root = doc.RootElement;

                if (root.TryGetProperty("videoId", out var videoId))
                {
                    return videoId.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("playlistId", out var playlistId))
                {
                    return playlistId.GetString() ?? string.Empty;
                }

                if (root.TryGetProperty("channelId", out var channelId))
                {
                    return channelId.GetString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
            => writer.WriteStringValue(value);
    }

}
