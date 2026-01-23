using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.X.Features.Tweets.Commands
{
    public class EnableTweetsSyncCommand : IRequest<CommandResponse>
    {
        public EnableTweetsSyncCommand(string accountName, string bearerToken)
        {
            AccountName = accountName;
            BearerToken = bearerToken;
        }

        public string AccountName { get; set; }

        public string BearerToken { get; set; }

    }
}
