using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class SignupOptions
    {
        [JsonPropertyName("connect")]
        public Connect Connect { get; set; }

        [JsonPropertyName("embed")]
        public Embed Embed { get; set; }

        [JsonPropertyName("public")]
        public Public Public { get; set; }

        [JsonPropertyName("sequence_signup")]
        public string SequenceSignup { get; set; }

        [JsonPropertyName("signup_cancel")]
        public string SignupCancel { get; set; }

        [JsonPropertyName("signup_enabled")]
        public string SignupEnabled { get; set; }

        [JsonPropertyName("tickets")]
        public Tickets Tickets { get; set; }

        [JsonPropertyName("visible_to_tags")]
        public List<string> VisibleToTags { get; set; }

        [JsonPropertyName("fixed_questions")]
        public FixedQuestions FixedQuestions { get; set; }
    }


}
