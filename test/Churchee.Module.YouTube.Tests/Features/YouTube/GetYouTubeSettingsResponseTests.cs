using Churchee.Module.YouTube.Features.YouTube.Queries;

namespace Churchee.Module.YouTube.Tests.Features.YouTube
{
    public class GetYouTubeSettingsResponseTests
    {
        [Fact]
        public void Constructor_Sets_Properties()
        {
            var now = DateTime.UtcNow;
            var response = new GetYouTubeSettingsResponse("handle", "name", now);

            Assert.Equal("handle", response.Handle);
            Assert.Equal("name", response.NameForContent);
            Assert.Equal(now, response.LastRun);
        }
    }
}
