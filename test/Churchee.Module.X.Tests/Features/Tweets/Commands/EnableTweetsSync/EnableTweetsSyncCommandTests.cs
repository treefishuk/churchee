using Churchee.Module.X.Features.Tweets.Commands;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.X.Tests.Features.Tweets.Commands.EnableTweetsSync
{
    public class EnableTweetsSyncCommandTests
    {
        [Fact]
        public void EnableTweetsSyncCommand_Correctly_Sets_Properties()
        {
            //arrange
            string accountName = "test_account";
            string bearerToken = "test_bearer_token";

            //act
            var cut = new EnableTweetsSyncCommand(accountName, bearerToken);

            //assert
            cut.AccountName.Should().Be(accountName);
            cut.BearerToken.Should().Be(bearerToken);
        }
    }
}
