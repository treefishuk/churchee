using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Site
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("initials")]
        public string Initials { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("address")]
        public Address Address { get; set; }

        [JsonPropertyName("mtime")]
        public string Mtime { get; set; }

        [JsonPropertyName("muser")]
        public string Muser { get; set; }

        [JsonPropertyName("ctime")]
        public string Ctime { get; set; }

        [JsonPropertyName("cuser")]
        public string Cuser { get; set; }
    }


}
