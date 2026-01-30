using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.Jotform.Features.Commands
{
    public class ConfigureJotformCommand : IRequest<CommandResponse>
    {
        public ConfigureJotformCommand(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string ApiKey { get; private set; }
    }
}
