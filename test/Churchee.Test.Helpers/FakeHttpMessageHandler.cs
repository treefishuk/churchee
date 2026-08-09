using System.Net;

namespace Churchee.Test.Helpers
{
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _statusCode;

        private readonly HttpResponseMessage[] _responseMessages;


        public int Increment { get; set; }

        public string RequestPath { get; set; }

        public FakeHttpMessageHandler(HttpStatusCode statusCode, params string[] responseContent)
        {
            _statusCode = statusCode;
            Increment = 0;
            RequestPath = string.Empty;
            _responseMessages = responseContent.Select(s => new HttpResponseMessage(_statusCode)
            {
                Content = new StringContent(s)
            }).ToArray();
        }

        public FakeHttpMessageHandler(params HttpResponseMessage[] responseContent)
        {
            _statusCode = HttpStatusCode.OK;
            Increment = 0;
            RequestPath = string.Empty;
            _responseMessages = responseContent;

        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestPath = request?.RequestUri?.ToString() ?? string.Empty;

            if (_responseMessages.Length == 0)
            {
                var emptyResponse = new HttpResponseMessage(_statusCode)
                {
                    Content = new StringContent(string.Empty)
                };

                return Task.FromResult(emptyResponse);
            }

            var response = _responseMessages[Increment];

            Increment++;

            return Task.FromResult(response);
        }
    }
}
