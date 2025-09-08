using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Helpers
{
    public class PageTypesTests
    {
        [Fact]
        public void VideoListingPageTypeId_IsExpectedGuid()
        {
            PageTypes.VideoListingPageTypeId.Should().Be(Guid.Parse("655a429f-c01a-4559-9c25-c2c450e7c14d"));
        }

        [Fact]
        public void VideoDetailPageTypeId_IsExpectedGuid()
        {
            PageTypes.VideoDetailPageTypeId.Should().Be(Guid.Parse("666e214e-bfe7-46ef-84ee-fdcda53c12c3"));
        }
    }
}
