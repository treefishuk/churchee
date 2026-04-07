using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Brand
    {
        [JsonPropertyName("brand_css")]
        public string BrandCss { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("emblem")]
        public string Emblem { get; set; }

        [JsonPropertyName("logo")]
        public string Logo { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("favicon_16")]
        public string Favicon16 { get; set; }

        [JsonPropertyName("favicon_32")]
        public string Favicon32 { get; set; }

        [JsonPropertyName("favicon_64")]
        public string Favicon64 { get; set; }

        [JsonPropertyName("favicon_128")]
        public string Favicon128 { get; set; }

        [JsonPropertyName("favicon_152")]
        public string Favicon152 { get; set; }

        [JsonPropertyName("favicon_200")]
        public string Favicon200 { get; set; }

        [JsonPropertyName("favicon_512")]
        public string Favicon512 { get; set; }

        [JsonPropertyName("email_font")]
        public string EmailFont { get; set; }

        [JsonPropertyName("email_footer")]
        public string EmailFooter { get; set; }
    }


}
