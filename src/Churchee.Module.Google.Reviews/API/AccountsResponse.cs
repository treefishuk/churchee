using System.Text.Json.Serialization;

namespace Churchee.Module.Google.Reviews.API
{
    public class AccountsResponse
    {
        [JsonPropertyName("accounts")]
        public List<GoogleAccount> Accounts { get; set; } = [];
    }
}
