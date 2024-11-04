using Churchee.Module.Dashboard.Entities;
using Churchee.Module.Dashboard.Middleware;
using Churchee.Module.Tenancy.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
            var dbContextMock = new Mock<DbContext>();
            var tenantResolverMock = new Mock<ITenantResolver>();
            var nextCalled = false;
            RequestDelegate next = (HttpContext _) =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            };
            var middleware = new UsageMiddleware(next);

            // Act
            await middleware.InvokeAsync(context, dbContextMock.Object, tenantResolverMock.Object);

            // Assert
            Assert.True(nextCalled);
        }

        [Fact]
        public async Task LogRequest_ShouldLogPageView()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Connection.RemoteIpAddress = IPAddress.Parse("127.0.0.1");
            context.Request.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:131.0) Gecko/20100101 Firefox/131.0";
            context.Request.Path = "/test";
            context.Request.Headers["Referer"] = "http://example.com";

            var dbContextMock = new Mock<DbContext>();
            var dbSetMock = new Mock<DbSet<PageView>>();
            dbContextMock.Setup(m => m.Set<PageView>()).Returns(dbSetMock.Object);

            var tenantResolverMock = new Mock<ITenantResolver>();
            tenantResolverMock.Setup(tr => tr.GetTenantId()).Returns(Guid.NewGuid());

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask);

            // Act
            await middleware.LogRequest(context, dbContextMock.Object, tenantResolverMock.Object);

            // Assert
            dbSetMock.Verify(m => m.Add(It.IsAny<PageView>()), Times.Once);
            dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task LogRequest_ShouldNotLogCssRequests()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Path = "/styles.css";

            var dbContextMock = new Mock<DbContext>();
            var tenantResolverMock = new Mock<ITenantResolver>();

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context, dbContextMock.Object, tenantResolverMock.Object);

            // Assert
            dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

        [Fact]
        public async Task LogRequest_ShouldNotLogBotRequests()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["User-Agent"] = "Googlebot";

            var dbContextMock = new Mock<DbContext>();
            var tenantResolverMock = new Mock<ITenantResolver>();

            var middleware = new UsageMiddleware((HttpContext _) => Task.CompletedTask);

            // Act
            await middleware.InvokeAsync(context, dbContextMock.Object, tenantResolverMock.Object);

            // Assert
            dbContextMock.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}

