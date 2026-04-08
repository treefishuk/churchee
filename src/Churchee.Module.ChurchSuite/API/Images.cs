using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Images
    {

        public Images()
        {
            Thumb = new Image();
            Small = new Image();
            Medium = new Image();
            Large = new Image();
        }

        [JsonPropertyName("thumb")]
        public Image Thumb { get; set; }

        [JsonPropertyName("sm")]
        public Image Small { get; set; }

        [JsonPropertyName("md")]
        public Image Medium { get; set; }

        [JsonPropertyName("lg")]
        public Image Large { get; set; }
    }

}
