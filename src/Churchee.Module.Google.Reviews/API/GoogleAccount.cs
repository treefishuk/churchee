using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class GoogleAccount
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("accountName")]
        public string AccountName { get; set; } = null!;
    }
}
