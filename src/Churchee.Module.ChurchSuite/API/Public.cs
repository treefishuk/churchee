using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Public
    {
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }

        [JsonPropertyName("enabled")]
        public string Enabled { get; set; }

        [JsonPropertyName("featured")]
        public bool Featured { get; set; }
    }


}
