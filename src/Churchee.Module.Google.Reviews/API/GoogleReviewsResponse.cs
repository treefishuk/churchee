using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class GoogleReviewsResponse
    {
        [JsonPropertyName("reviews")]
        public List<GoogleReview> Reviews { get; set; } = [];
    }
}
