using System.Text.Json.Serialization;

namespace Churchee.Module.X.Features.Tweets.Commands.EnableTweetsSync
{
    internal class GetAccountIdApiResponse
    {
        public GetAccountIdApiResponse()
        {
            Data = new Data();
        }

        [JsonPropertyName("data")]
        public Data Data { get; set; }

        public string GetId()
        {
            return Data == null ? string.Empty : Data.Id;
        }
    }

    internal class Data
    {
        public Data()
        {
            Id = string.Empty;
            Name = string.Empty;
            Username = string.Empty;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}

