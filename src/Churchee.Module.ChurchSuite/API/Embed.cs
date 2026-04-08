using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Embed
    {
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("enabled")]
        public string Enabled { get; set; }
    }


}
