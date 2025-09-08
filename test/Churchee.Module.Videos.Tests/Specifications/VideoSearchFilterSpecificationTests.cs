using Churchee.Module.Videos.Entities;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Specifications
{
    public class VideoSearchFilterSpecificationTests
    {
        [Fact]
        public void Specification_FiltersVideos_BySearchText()
        {
            // Arrange
            var videos = new List<Video>
            {
                new(
                    Guid.NewGuid(),
                    "https://example.com/video1",
                    DateTime.UtcNow,
                    "TestSource",
                    "SourceId1",
                    "Test Video",
                    "A test video description",
                    "https://example.com/thumb1.jpg",
                    "/videos/video1.mp4",
                    Guid.NewGuid()
                ),
                new(
                    Guid.NewGuid(),
                    "https://example.com/video2",
                    DateTime.UtcNow,
                    "TestSource",
                    "SourceId2",
                    "Another",
                    "Another video description",
                    "https://example.com/thumb2.jpg",
                    "/videos/video2.mp4",
                    Guid.NewGuid()
                )
            }.AsQueryable();

            // Act
            var spec = new VideoSearchFilterSpecification("Test");
            var predicate = spec.WhereExpressions.First().Filter.Compile();
            var filtered = videos.Where(predicate).ToList();

            // Assert
            filtered.Should().HaveCount(1);
            filtered.First().Title.Should().Be("Test Video");
        }
    }
}
