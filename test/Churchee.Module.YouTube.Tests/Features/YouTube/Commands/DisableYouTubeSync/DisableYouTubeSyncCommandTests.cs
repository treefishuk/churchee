using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;
using Churchee.Module.YouTube.Features.YouTube.Commands;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.DisableYouTubeSync
{
    public class DisableYouTubeSyncCommandTests
    {
        [Fact]
        public void Command_Is_Request_Of_CommandResponse()
        {
            var cmd = new DisableYouTubeSyncCommand();
            Assert.IsType<IRequest<CommandResponse>>(cmd, exactMatch: false);
        }
    }
}
