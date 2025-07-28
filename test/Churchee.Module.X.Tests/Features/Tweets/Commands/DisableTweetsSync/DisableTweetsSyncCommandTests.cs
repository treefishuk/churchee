using Churchee.Module.X.Features.Tweets.Commands;
using FluentAssertions;

namespace Churchee.Module.X.Tests.Features.Tweets.Commands.DisableTweetsSync
{
    public class DisableTweetsSyncCommandTests
    {
        [Fact]
        public void DisableTweetsSyncCommand_Correctly_Sets_ApplicationTenant()
        {
            //arrange
            var tenantId = Guid.NewGuid();

            //act
            var cut = new DisableTweetsSyncCommand(tenantId);

            //assert
            cut.ApplicationTenantId.Should().Be(tenantId);
        }
    }
}
