using Churchee.Common.ResponseTypes;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.EnableYouTubeSync
{
    public class EnableYouTubeSyncCommandTests
    {
        [Fact]
        public void Constructor_Sets_Properties()
        {
            var cmd = new EnableYouTubeSyncCommand("key", "handle");
            Assert.Equal("key", cmd.ApiKey);
            Assert.Equal("handle", cmd.Handle);
        }

        [Fact]
        public void Is_Request_Of_CommandResponse()
        {
            var cmd = new EnableYouTubeSyncCommand("key", "h");
            Assert.IsType<MediatR.IRequest<CommandResponse>>(cmd, exactMatch: false);
        }
    }
}
