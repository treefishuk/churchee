using Xunit;
using Churchee.Module.YouTube.Features.YouTube.Commands;
using Churchee.Common.ResponseTypes;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.DisableYouTubeSync
{
    public class DisableYouTubeSyncCommandTests
    {
        [Fact]
        public void Command_Is_Request_Of_CommandResponse()
        {
            var cmd = new DisableYouTubeSyncCommand();
            Assert.IsType<MediatR.IRequest<CommandResponse>>(cmd, exactMatch: false);
        }
    }
}
