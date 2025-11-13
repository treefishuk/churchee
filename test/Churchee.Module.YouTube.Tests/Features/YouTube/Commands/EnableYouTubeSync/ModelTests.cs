using System;
using Xunit;
using Churchee.Module.YouTube.Features.YouTube.Commands.EnableYouTubeSync;

namespace Churchee.Module.YouTube.Tests.Features.YouTube.Commands.EnableYouTubeSync
{
    public class ModelTests
    {
        [Fact]
        public void GetChannelIdApiResponse_Defaults()
        {
            var item = new Item();
            Assert.Equal(string.Empty, item.Id);

            var response = new GetChannelIdApiResponse();
            Assert.NotNull(response.Items);
        }

        [Fact]
        public void GetYouTubeVideosApiResponse_Defaults()
        {
            var response = new GetYouTubeVideosApiResponse();
            Assert.NotNull(response.Items);
        }

        [Fact]
        public void YouTubeVideo_Defaults()
        {
            var video = new YouTubeVideo();
            Assert.Equal(string.Empty, video.Kind);
            Assert.Equal(string.Empty, video.Etag);
            Assert.NotNull(video.Id);
            Assert.NotNull(video.Snippet);
        }

        [Fact]
        public void Snippet_Defaults()
        {
            var snippet = new Snippet();
            Assert.Equal(string.Empty, snippet.ChannelId);
            Assert.Equal(string.Empty, snippet.Title);
            Assert.NotNull(snippet.Thumbnails);
        }
    }
}
