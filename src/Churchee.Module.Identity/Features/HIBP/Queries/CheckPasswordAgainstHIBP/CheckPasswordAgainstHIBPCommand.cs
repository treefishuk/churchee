using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Identity.Features.HIBP.Queries
{
    public class CheckPasswordAgainstHIBPCommand : IRequest<CommandResponse>
    {
        public CheckPasswordAgainstHIBPCommand(string password)
        {
            Password = password;
        }

        public string Password { get; }
    }
}
