using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Identity.Features.HIBP.Queries
{
    public class CheckPasswordAgainstHibpCommand : IRequest<CommandResponse>
    {
        public CheckPasswordAgainstHibpCommand(string password)
        {
            Password = password;
        }

        public string Password { get; }
    }
}
