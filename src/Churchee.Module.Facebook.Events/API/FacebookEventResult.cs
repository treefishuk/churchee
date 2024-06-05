using System.Text.Json.Serialization;

namespace Churchee.Module.Facebook.Events.API
{
    public class Cover
    {
        [JsonPropertyName("offset_x")]
        public int OffsetX { get; set; }

        [JsonPropertyName("offset_y")]
        public int OffsetY { get; set; }

        [JsonPropertyName("source")]
        public string Source { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Location
    {
        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("zip")]
        public string Zip { get; set; }
    }

    public class Place
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("location")]
        public Location Location { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class FacebookEventResult
    {
        [JsonPropertyName("cover")]
        public Cover Cover { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("place")]
        public Place Place { get; set; }

        [JsonPropertyName("start_time")]
        public DateTime? StartTime { get; set; }

        [JsonPropertyName("end_time")]
        public DateTime? EndTime { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Cursors
    {
        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("after")]
        public string After { get; set; }
    }

    public class Paging
    {
        [JsonPropertyName("cursors")]
        public Cursors Cursors { get; set; }

        [JsonPropertyName("next")]
        public string Next { get; set; }
    }


}
