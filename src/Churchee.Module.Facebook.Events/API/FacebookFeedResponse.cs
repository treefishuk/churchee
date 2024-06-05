using System.Text.Json.Serialization;

namespace Churchee.Module.Facebook.Events.API
{
    public class FacebookFeedResponse
    {
        [JsonPropertyName("data")]
        public List<FacebookFeedResponseItem> Data { get; set; }

    }
}

