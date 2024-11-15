using Churchee.Module.Dashboard.Middleware;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace Churchee.Module.Dashboard.Tests.Middleware
{
    public class UsageMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldCallNextMiddleware()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = new Mock<ILogger<UsageMiddleware>>();
            var serviceProvider = new Mock<IServiceProvider>();

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
            .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            var dbContextMock = new Mock<DbContext>();
            var tenantResolverMock = new Mock<ITenantResolver>();
            var nextCalled = false;
            RequestDelegate next = (HttpContext _) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new UsageMiddleware(next, logger.Object, serviceProvider.Object);

            // Act
            await middleware.InvokeAsync(context, tenantResolverMock.Object);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task LogRequest_ShouldLogPageView()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = new Mock<ILogger<UsageMiddleware>>();
            var serviceProvider = new Mock<IServiceProvider>();

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
            .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);


            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:131.0) Gecko/20100101 Firefox/131.0";
            context.Request.Path = "/test";
            context.Request.Headers["Referer"] = "http://example.com";

            var tenantResolverMock = new Mock<ITenantResolver>();
            tenantResolverMock.Setup(tr => tr.GetTenantId()).Returns(Guid.NewGuid());

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask, logger.Object, serviceProvider.Object);

            // Act
            await middleware.LogRequest(context, tenantResolverMock.Object);

            // Assert
            tenantResolverMock.Verify(m => m.GetTenantId(), Times.Once);
        }

        [Fact]
        public async Task LogRequest_ShouldNotLogCssRequests()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/styles.css";

            var logger = new Mock<ILogger<UsageMiddleware>>();

            var serviceProvider = new Mock<IServiceProvider>();

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
            .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            var tenantResolverMock = new Mock<ITenantResolver>();

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask, logger.Object, serviceProvider.Object);

            // Act
            await middleware.InvokeAsync(context, tenantResolverMock.Object);

            // Assert
            tenantResolverMock.Verify(m => m.GetTenantId(), Times.Once);
        }

        [Fact]
        public async Task LogRequest_ShouldNotLogBotRequests()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["User-Agent"] = "Googlebot";

            var logger = new Mock<ILogger<UsageMiddleware>>();
            var serviceProvider = new Mock<IServiceProvider>();

            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            serviceScopeFactory
            .Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);

            serviceProvider
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactory.Object);

            var tenantResolverMock = new Mock<ITenantResolver>();

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask, logger.Object, serviceProvider.Object);

            // Act
            await middleware.InvokeAsync(context, tenantResolverMock.Object);

            // Assert
            tenantResolverMock.Verify(m => m.GetTenantId(), Times.Once);
        }
    }
}

