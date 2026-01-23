using Churchee.Module.Site.Features.Blog.Queries;

namespace Churchee.Module.Site.Tests.Features.Blog.Queries.GetArticleById
{
    public class GetArticleByIdResponseTests
    {
        [Fact]
        public void CanSetAndGetProperties()
        {
            // Arrange
            var response = new GetArticleByIdResponse
            {
                Id = Guid.NewGuid(),
                Title = "Test Title",
                Content = "Test Content",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Description = "desc",
                ParentId = Guid.NewGuid(),
                ParentName = "Parent",
                IsPublished = true,
                PublishOnDate = DateTime.UtcNow
            };

            // Assert
            Assert.Equal("Test Title", response.Title);
            Assert.Equal("Test Content", response.Content);
            Assert.NotNull(response.CreatedAt);
            Assert.NotNull(response.UpdatedAt);
            Assert.Equal("desc", response.Description);
            Assert.NotNull(response.ParentId);
            Assert.Equal("Parent", response.ParentName);
            Assert.True(response.IsPublished);
            Assert.NotNull(response.PublishOnDate);
        }
    }
}
