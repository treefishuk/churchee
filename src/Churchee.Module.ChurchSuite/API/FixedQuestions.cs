using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class FixedQuestions
    {
        [JsonPropertyName("name")]
        public Title Name { get; set; }

        [JsonPropertyName("email")]
        public Email Email { get; set; }

        [JsonPropertyName("mobile")]
        public Mobile Mobile { get; set; }

        [JsonPropertyName("notes")]
        public Notes Notes { get; set; }
    }


}
