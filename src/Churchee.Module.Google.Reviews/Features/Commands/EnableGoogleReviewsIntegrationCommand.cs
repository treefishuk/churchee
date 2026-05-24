using Churchee.Common.ResponseTypes;
using Churchee.CQRS.Abstractions;

namespace Churchee.Module.Google.Reviews.Features.Commands
{
    public class EnableGoogleReviewsIntegrationCommand : IRequest<CommandResponse>
    {
        public EnableGoogleReviewsIntegrationCommand(string code, string domain)
        {
            Code = code;
            Domain = domain;
        }

        public string Code { get; set; }
        public string Domain { get; set; }

    }
}
