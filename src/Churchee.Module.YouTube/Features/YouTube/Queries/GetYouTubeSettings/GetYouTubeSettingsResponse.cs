namespace Churchee.Module.YouTube.Features.YouTube.Queries
{
    public class GetYouTubeSettingsResponse
    {
        public GetYouTubeSettingsResponse(string handle, string nameForContent, DateTime? lastRun)
        {
            Handle = handle;
            NameForContent = nameForContent;
            LastRun = lastRun;
        }

        public string Handle { get; private set; }

        public string NameForContent { get; private set; }

        public DateTime? LastRun { get; private set; }

    }
}
