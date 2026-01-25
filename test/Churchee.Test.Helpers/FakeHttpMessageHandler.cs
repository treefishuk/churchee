using System.Net;

namespace Churchee.Test.Helpers
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;
        private readonly string[] _responseContent;

        public int Increment { get; set; }

        public string RequestPath { get; set; }

        public FakeHttpMessageHandler(HttpStatusCode statusCode, params string[] responseContent)
        {
            _statusCode = statusCode;
            _responseContent = responseContent;
            Increment = 0;
            RequestPath = string.Empty;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestPath = request?.RequestUri?.ToString() ?? string.Empty;

            if (_responseContent.Length == 0)
            {
                var emptyResponse = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(string.Empty)
                };

                return Task.FromResult(emptyResponse);
            }

            var response = new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(_responseContent[Increment])
            };

            Increment++;

            return Task.FromResult(response);
        }
    }
}
