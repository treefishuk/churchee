using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class LocationsResponse
    {
        [JsonPropertyName("locations")]
        public List<Location> Locations { get; set; } = [];

    }
}
