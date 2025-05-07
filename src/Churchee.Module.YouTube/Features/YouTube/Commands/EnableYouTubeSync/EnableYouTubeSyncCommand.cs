using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.YouTube.Spotify.Features.YouTube.Commands
{
    public class EnableYouTubeSyncCommand : IRequest<CommandResponse>
    {
        public EnableYouTubeSyncCommand(string apiKey, string handle)
        {
            ApiKey = apiKey;
            Handle = handle;
        }

        public string ApiKey { get; private set; }

        public string Handle { get; private set; }
    }
}
