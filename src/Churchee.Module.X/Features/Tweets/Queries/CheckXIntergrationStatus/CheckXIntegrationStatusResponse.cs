namespace Churchee.Module.X.Features.Tweets.Queries
{
    public class CheckXIntegrationStatusResponse
    {
        public bool Configured { get; set; }

        public DateTime? LastRun { get; set; }

    }
}
