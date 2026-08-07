using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.YouTube.Spotify.Features.YouTube.Commands
{
    public class EnableYouTubeSyncCommand : IRequest<CommandResponse>
    {
        public EnableYouTubeSyncCommand(string apiKey, string handle, string playlistId)
        {
            ApiKey = apiKey;
            ChannelIdentifier = handle;
            PlaylistId = playlistId;
        }

        public string ApiKey { get; private set; }

        public string ChannelIdentifier { get; private set; }

        public string PlaylistId { get; private set; }


    }
}
