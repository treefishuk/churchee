using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class GoogleReviewer
    {
        [JsonPropertyName("displayName")]
        public string DisplayName { get; set; } = null!;

        [JsonPropertyName("profilePhotoUrl")]
        public string ProfilePhotoUrl { get; set; }
    }
}
