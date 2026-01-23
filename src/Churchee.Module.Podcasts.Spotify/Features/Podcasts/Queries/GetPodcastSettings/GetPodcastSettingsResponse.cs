namespace Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries
{
    public class GetPodcastSettingsResponse
    {
        public GetPodcastSettingsResponse(string spotifyUrl, string nameForContent, DateTime? lastRun)
        {
            SpotifyUrl = spotifyUrl;
            NameForContent = nameForContent;
            LastRun = lastRun;
        }

        public string SpotifyUrl { get; private set; }

        public string NameForContent { get; private set; }

        public DateTime? LastRun { get; private set; }

    }
}
