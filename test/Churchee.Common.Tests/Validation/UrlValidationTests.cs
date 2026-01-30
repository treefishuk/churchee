using Churchee.Common.Validation;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Common.Tests.Validation
{
    public class UrlValidationTests
    {
        [Theory]
        [InlineData("http://example.com", false)]
        [InlineData("https://example.com", false)]
        [InlineData("https://www.example.com", false)]
        [InlineData("https://subdomain.example.com/path?query=string#fragment", false)]
        [InlineData("/", true)]
        [InlineData("/home", true)]
        [InlineData("/some-page", true)]
        [InlineData("/some-page/child", true)]
        [InlineData("//evil.com", false)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void UrlValidation_Tests(string? url, bool expectedResult)
        {
            UrlValidation.IsSafeRelativeUrl(url).Should().Be(expectedResult);
        }

    }
}
