using Churchee.Common.ResponseTypes;
using MediatR;

namespace Churchee.Module.ChurchSuite.Features.Commands.EnableChurchSuiteIntegration
{
    public class EnableChurchSuiteIntegrationCommand : IRequest<CommandResponse>
    {
        public EnableChurchSuiteIntegrationCommand(string subdomain)
        {
            Url = $"https://{subdomain}/embed/calendar/json";
        }

        public string Url { get; set; }
    }
}
