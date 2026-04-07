using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Address
    {
        [JsonPropertyName("id")]
        public object Id { get; set; }

        [JsonPropertyName("line1")]
        public string Line1 { get; set; }

        [JsonPropertyName("line2")]
        public string Line2 { get; set; }

        [JsonPropertyName("line3")]
        public string Line3 { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("county")]
        public string County { get; set; }

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }
    }


}
