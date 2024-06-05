using System.Text.Json.Serialization;

namespace Churchee.Module.Facebook.Events.API
{
    public class FaceboookPageResponseItem
    {

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("id")]

        public string Id { get; set; }
    }
}
