using System.Text.Json.Serialization;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync
{

    public class GetYouTubeVideosApiResponse
    {
        [JsonPropertyName("items")]
        public List<YouTubeVideo> Items { get; set; }
    }

    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Default
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class High
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Id
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }
    }

    public class YouTubeVideo
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("etag")]
        public string Etag { get; set; }

        [JsonPropertyName("id")]
        public Id Id { get; set; }

        [JsonPropertyName("snippet")]
        public Snippet Snippet { get; set; }
    }

    public class Medium
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }


    public class Snippet
    {
        [JsonPropertyName("publishedAt")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonPropertyName("channelTitle")]
        public string ChannelTitle { get; set; }

        [JsonPropertyName("liveBroadcastContent")]
        public string LiveBroadcastContent { get; set; }

        [JsonPropertyName("publishTime")]
        public DateTime PublishTime { get; set; }
    }

    public class Thumbnails
    {
        [JsonPropertyName("default")]
        public Default Default { get; set; }

        [JsonPropertyName("medium")]
        public Medium Medium { get; set; }

        [JsonPropertyName("high")]
        public High High { get; set; }
    }


}
