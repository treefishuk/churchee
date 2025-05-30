using System.Text.Json.Serialization;

namespace Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync
{

    public class GetYouTubeVideosApiResponse
    {
        public GetYouTubeVideosApiResponse()
        {
            Items = new List<YouTubeVideo>();
        }

        [JsonPropertyName("items")]
        public List<YouTubeVideo> Items { get; set; }
    }

    public class Default
    {
        public Default()
        {
            Url = string.Empty;
        }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class High
    {
        public High()
        {
            Url = string.Empty;
        }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    public class Id
    {
        public Id()
        {
            Kind = string.Empty;
            VideoId = string.Empty;
        }

        [JsonPropertyName("kind")]
        public string Kind { get; set; }

        [JsonPropertyName("videoId")]
        public string VideoId { get; set; }
    }

    public class YouTubeVideo
    {
        public YouTubeVideo()
        {
            Kind = string.Empty;
            Etag = string.Empty;
            Id = new Id();
            Snippet = new Snippet();
        }

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
        public Medium()
        {
            Url = string.Empty;
        }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }


    public class Snippet
    {
        public Snippet()
        {
            ChannelId = string.Empty;
            Title = string.Empty;
            Description = string.Empty;
            ChannelTitle = string.Empty;
            Thumbnails = new Thumbnails();
            LiveBroadcastContent = string.Empty;
        }

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
        public Thumbnails()
        {
            Default = new Default();
            Medium = new Medium();
            High = new High();
        }

        [JsonPropertyName("default")]
        public Default Default { get; set; }

        [JsonPropertyName("medium")]
        public Medium Medium { get; set; }

        [JsonPropertyName("high")]
        public High High { get; set; }
    }


}
