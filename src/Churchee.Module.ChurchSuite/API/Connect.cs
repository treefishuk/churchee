using System.Text.Json.Serialization;

namespace Churchee.Module.ChurchSuite.API
{
    public class Connect
    {
        [JsonPropertyName("visible")]
        public bool Visible { get; set; }
    }


}
