using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Churchee.Module.Facebook.Events.API
{
    public class FacebookFeedResponseItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("story")]
        public string Story { get; set; }

    }
}
