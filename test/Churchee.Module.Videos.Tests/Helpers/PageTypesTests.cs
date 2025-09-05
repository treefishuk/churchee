using System;
using Xunit;
using Churchee.Module.Videos.Helpers;

namespace Churchee.Module.Videos.Helpers
{
    public class PageTypesTests
    {
        [Fact]
        public void VideoListingPageTypeId_IsExpectedGuid()
        {
            Assert.Equal(Guid.Parse("655a429f-c01a-4559-9c25-c2c450e7c14d"), PageTypes.VideoListingPageTypeId);
        }

        [Fact]
        public void VideoDetailPageTypeId_IsExpectedGuid()
        {
            Assert.Equal(Guid.Parse("666e214e-bfe7-46ef-84ee-fdcda53c12c3"), PageTypes.VideoDetailPageTypeId);
        }
    }
}
