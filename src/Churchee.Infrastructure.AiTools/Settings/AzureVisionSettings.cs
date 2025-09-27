namespace Churchee.Infrastructure.AiTools.Settings
{
    public class AzureVisionSettings
    {
        public AzureVisionSettings()
        {
            Endpoint = string.Empty;
            ApiKey = string.Empty;
        }

        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }
}
