using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Image
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
