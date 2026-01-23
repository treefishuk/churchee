using Churchee.Module.Dashboard.Entities;

namespace Churchee.Module.Dashboard.Tests.Entities
{
    public class PageViewTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();

            // Act
            var pageView = new PageView(applicationTenantId);

            // Assert
            Assert.Equal(applicationTenantId, pageView.ApplicationTenantId);
            Assert.Equal(default, pageView.IpAddress);
            Assert.Equal(default, pageView.UserAgent);
            Assert.Equal(default, pageView.Url);
            Assert.Equal(default, pageView.Device);
            Assert.Equal(default, pageView.OS);
            Assert.Equal(default, pageView.Browser);
            Assert.Equal(default, pageView.Referrer);
            Assert.Equal(default, pageView.ViewedAt);
        }

        [Fact]
        public void Properties_ShouldSetAndGetValues()
        {
            // Arrange
            var applicationTenantId = Guid.NewGuid();
            var pageView = new PageView(applicationTenantId);
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var url = "http://example.com";
            var device = "Desktop";
            var os = "Windows";
            var browser = "Chrome";
            var referrer = "http://google.com";
            var viewedAt = DateTime.UtcNow;

            // Act
            pageView.IpAddress = ipAddress;
            pageView.UserAgent = userAgent;
            pageView.Url = url;
            pageView.Device = device;
            pageView.OS = os;
            pageView.Browser = browser;
            pageView.Referrer = referrer;
            pageView.ViewedAt = viewedAt;

            // Assert
            Assert.Equal(ipAddress, pageView.IpAddress);
            Assert.Equal(userAgent, pageView.UserAgent);
            Assert.Equal(url, pageView.Url);
            Assert.Equal(device, pageView.Device);
            Assert.Equal(os, pageView.OS);
            Assert.Equal(browser, pageView.Browser);
            Assert.Equal(referrer, pageView.Referrer);
            Assert.Equal(viewedAt, pageView.ViewedAt);
        }
    }
}
