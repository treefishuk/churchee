using Churchee.Module.Reviews.Entities;

namespace Churchee.Module.Reviews.Tests.Entities
{
    public class ReviewTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            int rating = 5;
            string reviewerName = "John Doe";
            string reviewerImageUrl = "http://example.com/image.jpg";
            string comment = "Great product!";
            string sourceName = "Google";
            string sourceId = "12345";

            // Act
            var review = new Review(tenantId)
            {
                Rating = rating,
                ReviewerName = reviewerName,
                ReviewerImageUrl = reviewerImageUrl,
                Comment = comment,
                SourceName = sourceName,
                SourceId = sourceId
            };

            // Assert
            Assert.Equal(rating, review.Rating);
            Assert.Equal(reviewerName, review.ReviewerName);
            Assert.Equal(reviewerImageUrl, review.ReviewerImageUrl);
            Assert.Equal(comment, review.Comment);
            Assert.Equal(sourceName, review.SourceName);
            Assert.Equal(sourceId, review.SourceId);

        }
    }
}
