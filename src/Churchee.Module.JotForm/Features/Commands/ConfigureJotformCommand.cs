using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

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
