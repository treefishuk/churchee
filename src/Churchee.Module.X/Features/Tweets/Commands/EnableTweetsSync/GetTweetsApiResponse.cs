using System.Text.Json.Serialization;

namespace Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync
{
    public class Tweet
    {
        public Tweet()
        {
            Text = string.Empty;
            Id = string.Empty;
        }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }

    public class GetTweetsApiResponse
    {
        public GetTweetsApiResponse()
        {
            Tweets = new List<Tweet>();
        }

        [JsonPropertyName("data")]
        public List<Tweet> Tweets { get; set; }
    }

}
