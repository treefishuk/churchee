using Churchee.Module.Videos.Features.Queries;
using Churchee.Test.Helpers.Validation;

namespace Churchee.Module.Videos.Tests.Features.Queries.GetListing
{
    public class GetListingQueryResponseItemTests
    {
        [Fact]
        public void Constructor_InitializesStringPropertiesToEmpty()
        {
            // Act
            var item = new GetListingQueryResponseItem();

            // Assert
            item.ImageUri.Should().BeEmpty();
            item.Title.Should().BeEmpty();
            item.Source.Should().BeEmpty();
        }

        [Fact]
        public void Properties_CanBeSetAndRetrieved()
        {
            // Arrange
            var id = Guid.NewGuid();
            string imageUri = "/img/video.png";
            string title = "Test Video";
            string source = "YouTube";
            bool active = true;
            var publishedDate = new DateTime(2024, 1, 1);

            // Act
            var item = new GetListingQueryResponseItem
            {
                Id = id,
                ImageUri = imageUri,
                Title = title,
                Source = source,
                Active = active,
                PublishedDate = publishedDate
            };

            // Assert
            item.Id.Should().Be(id);
            item.ImageUri.Should().Be(imageUri);
            item.Title.Should().Be(title);
            item.Source.Should().Be(source);
            item.Active.Should().BeTrue();
            item.PublishedDate.Should().Be(publishedDate);
        }
    }
}