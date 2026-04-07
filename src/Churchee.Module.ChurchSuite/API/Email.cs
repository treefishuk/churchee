using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Email
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("response_type")]
        public string ResponseType { get; set; }

        [JsonPropertyName("required")]
        public string Required { get; set; }

        [JsonPropertyName("hidden")]
        public bool Hidden { get; set; }
    }


}
