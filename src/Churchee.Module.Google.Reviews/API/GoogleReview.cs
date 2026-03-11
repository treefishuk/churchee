using Churchee.Module.Google.Reviews.Helpers;
using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class GoogleReview
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("comment")]
        public string Comment { get; set; } = null!;

        [JsonPropertyName("starRating")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public StarRating StarRating { get; set; }

        [JsonPropertyName("reviewer")]
        public GoogleReviewer Reviewer { get; set; } = null!;

        [JsonPropertyName("createTime")]
        public DateTime CreateTime { get; set; }
    }
}
