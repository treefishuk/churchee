using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class Location
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("title")]
        public string Title { get; set; } = null!;
    }
}
