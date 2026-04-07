using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class ApiResponse
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }

        [JsonPropertyName("sequence")]
        public int? Sequence { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("datetime_start")]
        public string DatetimeStart { get; set; }

        [JsonPropertyName("datetime_end")]
        public string DatetimeEnd { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("category")]
        public Category Category { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("visible_to")]
        public List<string> VisibleTo { get; set; }

        [JsonPropertyName("brand")]
        public Brand Brand { get; set; }

        [JsonPropertyName("capacity")]
        public int? Capacity { get; set; }

        //[JsonPropertyName("images")]
        //public Images Images { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("signup_options")]
        public SignupOptions SignupOptions { get; set; }

        [JsonPropertyName("site")]
        public Site Site { get; set; }

        [JsonPropertyName("site_ids")]
        public List<int> SiteIds { get; set; }

        [JsonPropertyName("pin")]
        public int Pin { get; set; }

        [JsonPropertyName("public_visible")]
        public bool PublicVisible { get; set; }

        [JsonPropertyName("mtime")]
        public string Mtime { get; set; }

        [JsonPropertyName("muser")]
        public string Muser { get; set; }

        [JsonPropertyName("ctime")]
        public string Ctime { get; set; }

        [JsonPropertyName("cuser")]
        public string Cuser { get; set; }

        [JsonPropertyName("merged_by_strategy")]
        public bool MergedByStrategy { get; set; }
    }


}
