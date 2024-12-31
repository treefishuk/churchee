using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Spotify.Features.Podcasts.Queries;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Podcasts.Spotify.Tests.Features.Podcasts.Queries.GetPodcastSettings
{
    public class GetPodcastSettingsRequestHandlerTests
    {

        private readonly GetPodcastSettingsRequestHandler _handler;
        private readonly Mock<ISettingStore> _settingStoreMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<IJobService> _jobServiceMock;


        public GetPodcastSettingsRequestHandlerTests()
        {
            _settingStoreMock = new Mock<ISettingStore>();
            _currentUserMock = new Mock<ICurrentUser>();
            _jobServiceMock = new Mock<IJobService>();

            _handler = new GetPodcastSettingsRequestHandler(
                _settingStoreMock.Object,
                _currentUserMock.Object,
                _jobServiceMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectResponse()
        {
            //arrange
            var tenantId = Guid.NewGuid();
            var lastRun = DateTime.Now;
            string pageName = "Page Name";
            string spotifyUrl = "http://example.com/rss";

            _currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(tenantId);
            _settingStoreMock.Setup(s => s.GetSettingValue(Guid.Parse("a9cd25bb-23b4-45ba-9484-04fc458ad29a"), tenantId)).ReturnsAsync(spotifyUrl);
            _settingStoreMock.Setup(s => s.GetSettingValue(Guid.Parse("4379e3d3-fa40-489b-b80d-01c30835fa9d"), tenantId)).ReturnsAsync(pageName);
            _jobServiceMock.Setup(s => s.GetLastRunDate($"{tenantId}_SpotifyPodcasts")).Returns(lastRun);

            //act
            var response = await _handler.Handle(new GetPodcastSettingsRequest(), CancellationToken.None);

            //assert
            response.LastRun.Should().Be(lastRun);
            response.SpotifyUrl.Should().Be(spotifyUrl);
            response.NameForContent.Should().Be(pageName);
        }
    }
}
