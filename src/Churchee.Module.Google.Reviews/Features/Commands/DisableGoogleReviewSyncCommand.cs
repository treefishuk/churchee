using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class DisableGoogleReviewSyncCommand : IRequest<CommandResponse>
    {
    }
}
