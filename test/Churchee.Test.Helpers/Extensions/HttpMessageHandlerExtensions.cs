using Moq;
using Moq.Protected;

namespace Churchee.Test.Helpers.Extensions
{
    public static class HttpMessageHandlerExtensions
    {
        public static void VerifyPost(
            this Mock<HttpMessageHandler> handler,
            string urlContains,
            Times times)
        {
            handler.Protected().Verify(
                "SendAsync",
                times,
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Post &&
                    req.RequestUri!.ToString().Contains(urlContains)),
                ItExpr.IsAny<CancellationToken>());
        }
    }

}
