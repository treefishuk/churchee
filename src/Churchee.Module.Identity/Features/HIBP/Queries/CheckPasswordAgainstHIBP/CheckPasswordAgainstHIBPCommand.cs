using Churchee.Common.ResponseTypes;
using MediatR;

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
