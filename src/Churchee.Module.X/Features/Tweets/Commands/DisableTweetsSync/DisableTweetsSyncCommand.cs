using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.X.Features.Tweets.Commands
{
    public class DisableTweetsSyncCommand : IRequest<CommandResponse>
    {
        public DisableTweetsSyncCommand(Guid applicationTenantId)
        {
            ApplicationTenantId = applicationTenantId;
        }

        public Guid ApplicationTenantId { get; set; }
    }
}
