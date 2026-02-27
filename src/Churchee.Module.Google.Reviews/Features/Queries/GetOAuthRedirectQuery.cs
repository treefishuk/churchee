using MediatR;

namespace Churchee.Module.Google.Reviews.Features.Queries
{
    public class GetOAuthRedirectQuery : IRequest<string>
    {
        public GetOAuthRedirectQuery(string domain, string clientId, string clientSecret, string businessProfileId)
        {
            Domain = domain;
            ClientId = clientId;
            ClientSecret = clientSecret;
            BusinessProfileId = businessProfileId;
        }

        public string Domain { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BusinessProfileId { get; set; }
    }
}
