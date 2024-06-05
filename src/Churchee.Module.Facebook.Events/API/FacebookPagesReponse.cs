using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Churchee.Module.Facebook.Events.Features.Commands.EnableFacebookIntegrationCommandHandler;

namespace Churchee.Module.Facebook.Events.API
{
    public class FacebookPagesReponse
    {
        [JsonPropertyName("data")]
        public List<FaceboookPageResponseItem> Data { get; set; }
    }
}
