using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Tickets
    {
        [JsonPropertyName("enabled")]
        public string Enabled { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
