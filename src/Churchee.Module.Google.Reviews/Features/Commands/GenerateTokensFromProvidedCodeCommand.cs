using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class GenerateTokensFromProvidedCodeCommand : IRequest<CommandResponse>
    {
        public GenerateTokensFromProvidedCodeCommand(string code, string domain)
        {
            Code = code;
            Domain = domain;
        }

        public string Code { get; set; }
        public string Domain { get; set; }

    }
}
