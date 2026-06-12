using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.EnableYouTubeSync
{
    public class EnableYouTubeSyncCommandTests
    {
        [Fact]
        public void Constructor_Sets_Properties()
        {
            var cmd = new EnableYouTubeSyncCommand("key", "handle", "playlist");
            Assert.Equal("key", cmd.ApiKey);
            Assert.Equal("handle", cmd.ChannelIdentifier);
            Assert.Equal("playlist", cmd.PlaylistId);
        }

        [Fact]
        public void Is_Request_Of_CommandResponse()
        {
            var cmd = new EnableYouTubeSyncCommand("key", "h", "playlist");
            Assert.IsType<IRequest<CommandResponse>>(cmd, exactMatch: false);
        }
    }
}
