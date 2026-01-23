using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using Churchee.Module.YouTube.Spotify.Features.YouTube.Commands;
using Churchee.Common.Abstractions.Storage;
using System.Linq.Expressions;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.EnableYouTubeSync
{
    public class EnableYouTubeSyncCommandHandlerTests
    {
        private class TestHandler : DelegatingHandler
        {
            private readonly HttpResponseMessage _response;

            public TestHandler(HttpResponseMessage response)
            {
                _response = response;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_response);
            }
        }

        [Fact]
        public async Task Handle_ReturnsErrorResponse_When_ChannelApiReturnsNonSuccess()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var cmd = new EnableYouTubeSyncCommand("apiKey", "handle");

            var settingStore = new Mock<ISettingStore>();
            settingStore.Setup(s => s.AddOrUpdateSetting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            settingStore.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync(string.Empty);

            var currentUser = new Mock<ICurrentUser>();
            currentUser.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var tokenRepo = new Mock<IRepository<Token>>();
            tokenRepo.Setup(r => r.Create(It.IsAny<Token>()));

            var dataStore = new Mock<IDataStore>();
            dataStore.Setup(d => d.GetRepository<Token>()).Returns(tokenRepo.Object);
            dataStore.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var jobService = new Mock<IJobService>();

            var badResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var handler = new TestHandler(badResponse);
            var client = new HttpClient(handler);

            var httpFactory = new Mock<IHttpClientFactory>();
            httpFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = new Mock<ILogger<EnableYouTubeSyncCommandHandler>>();

            var handlerInstance = new EnableYouTubeSyncCommandHandler(settingStore.Object, currentUser.Object, dataStore.Object, jobService.Object, httpFactory.Object, logger.Object);

            // Act
            var result = await handlerInstance.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            // Ensure we did not schedule jobs
            jobService.Verify(j => j.ScheduleJob(It.IsAny<string>(), It.IsAny<Expression<Func<Task>>>(), It.IsAny<Func<string>>()), Times.Never);
            jobService.Verify(j => j.QueueJob(It.IsAny<Expression<Func<Task>>>()), Times.Never);
        }

        [Fact]
        public async Task Handle_SchedulesJobs_And_ReturnsSuccess_On_ValidChannelId()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var cmd = new EnableYouTubeSyncCommand("apiKey", "handle");

            var settingStore = new Mock<ISettingStore>();
            settingStore.Setup(s => s.AddOrUpdateSetting(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            settingStore.Setup(s => s.GetSettingValue(It.IsAny<Guid>(), It.IsAny<Guid>())).ReturnsAsync("videos");

            var currentUser = new Mock<ICurrentUser>();
            currentUser.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var tokenRepo = new Mock<IRepository<Token>>();
            tokenRepo.Setup(r => r.Create(It.IsAny<Token>()));

            var dataStore = new Mock<IDataStore>();
            dataStore.Setup(d => d.GetRepository<Token>()).Returns(tokenRepo.Object);
            dataStore.Setup(d => d.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var jobService = new Mock<IJobService>();
            jobService.Setup(j => j.ScheduleJob(It.IsAny<string>(), It.IsAny<Expression<Func<Task>>>(), It.IsAny<Func<string>>()));
            jobService.Setup(j => j.QueueJob(It.IsAny<Expression<Func<Task>>>()));

            // Prepare a successful channel id response
            var apiResponse = new { items = new[] { new { id = "UC123" } } };
            var json = JsonSerializer.Serialize(new { channelId = "UC123", Items = new[] { new { id = "UC123" } } });
            var goodResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json)
            };

            var handler = new TestHandler(goodResponse);
            var client = new HttpClient(handler);

            var httpFactory = new Mock<IHttpClientFactory>();
            httpFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

            var logger = new Mock<ILogger<EnableYouTubeSyncCommandHandler>>();

            var handlerInstance = new EnableYouTubeSyncCommandHandler(settingStore.Object, currentUser.Object, dataStore.Object, jobService.Object, httpFactory.Object, logger.Object);

            // Act
            var result = await handlerInstance.Handle(cmd, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            jobService.Verify(j => j.ScheduleJob(It.IsAny<string>(), It.IsAny<Expression<Func<Task>>>(), It.IsAny<Func<string>>()), Times.Once);
            jobService.Verify(j => j.QueueJob(It.IsAny<Expression<Func<Task>>>()), Times.Once);
        }
    }
}
