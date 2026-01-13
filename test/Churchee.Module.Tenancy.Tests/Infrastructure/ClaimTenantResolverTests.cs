using Churchee.Module.Tenancy.Infrastructure;
using Churchee.Test.Helpers.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Security.Claims;

namespace Churchee.Module.Tenancy.Tests.Infrastructure
{
    public class ClaimTenantResolverTests
    {
        [Fact]
        public void GetTenantId_ReturnsEmpty_When_HttpContextIsNull()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            var result = resolver.GetTenantId();

            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetTenantId_ReturnsEmpty_When_ClaimMissing()
        {
            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // no claims
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            var result = resolver.GetTenantId();

            result.Should().Be(Guid.Empty);
        }

        [Fact]
        public void GetTenantId_ReturnsGuid_When_ClaimPresent()
        {
            var expected = Guid.NewGuid();
            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("ActiveTenantId", expected.ToString()) }))
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            var result = resolver.GetTenantId();

            result.Should().Be(expected);
        }

        [Fact]
        public void GetTenantDevName_ReturnsEmpty_When_HttpContextIsNull()
        {
            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns((HttpContext?)null);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            string result = resolver.GetTenantDevName();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetTenantDevName_ReturnsEmpty_When_ClaimMissing()
        {
            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity()) // no claims
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            string result = resolver.GetTenantDevName();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetTenantDevName_ReturnsLowerInvariant_When_ClaimPresent()
        {
            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("ActiveTenantName", "MyTENANT") }))
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var configMock = new Mock<IConfiguration>();

            var resolver = new ClaimTenantResolver(accessorMock.Object, configMock.Object);

            string result = resolver.GetTenantDevName();

            result.Should().Be("mytenant");
        }

        [Fact]
        public void GetCDNPrefix_ReplacesAsteriskWithTenantDevName()
        {
            // arrange
            string prefixTemplate = "https://cdn.example.com/*/assets";

            var inMemorySettings = new Dictionary<string, string?> {
                {"Images:Prefix", prefixTemplate}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("ActiveTenantName", "MyTENANT") }))
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var resolver = new ClaimTenantResolver(accessorMock.Object, configuration);

            // act
            string result = resolver.GetCDNPrefix();

            // assert
            result.Should().Be("https://cdn.example.com/mytenant/assets");
        }

        [Fact]
        public void GetCDNPrefix_ThrowsException_When_ConfigMissing()
        {
            var inMemorySettings = new Dictionary<string, string?>();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("ActiveTenantName", "X") }))
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var resolver = new ClaimTenantResolver(accessorMock.Object, configuration);

            Assert.Throws<InvalidOperationException>(resolver.GetCDNPrefix);
        }

        [Fact]
        public void GetCDNPrefix_ReturnsEmptyPrefix_When_ConfigEmpty()
        {


            var inMemorySettings = new Dictionary<string, string?> {
                {"Images:Prefix", ""}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var ctx = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("ActiveTenantName", "X") }))
            };

            var accessorMock = new Mock<IHttpContextAccessor>();
            accessorMock.Setup(a => a.HttpContext).Returns(ctx);

            var resolver = new ClaimTenantResolver(accessorMock.Object, configuration);

            string result = resolver.GetCDNPrefix();

            result.Should().BeEmpty();
        }
    }
}