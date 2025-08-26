using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Videos.Entities;
using Churchee.Module.Videos.Specifications;
using Moq;

namespace Churchee.Module.Videos.Features.Queries
{
    public class GetListingQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsDataTableResponse_WithExpectedItems()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Video>>();
            var expectedResponse = new DataTableResponse<GetListingQueryResponseItem>
            {
                Draw = 1,
                RecordsTotal = 2,
                RecordsFiltered = 2,
                Data = new List<GetListingQueryResponseItem>
            {
                new GetListingQueryResponseItem
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    ImageUri = "thumb1.jpg",
                    Title = "Video 1",
                    PublishedDate = DateTime.UtcNow,
                    Source = "YouTube"
                },
                new GetListingQueryResponseItem
                {
                    Id = Guid.NewGuid(),
                    Active = true,
                    ImageUri = "thumb2.jpg",
                    Title = "Video 2",
                    PublishedDate = DateTime.UtcNow,
                    Source = "Vimeo"
                }
            }
            };

            mockRepo.Setup(r => r.GetDataTableResponseAsync(
                    It.IsAny<VideoSearchFilterSpecification>(),
                    "Title",
                    "asc",
                    0,
                    10,
                    It.IsAny<System.Linq.Expressions.Expression<Func<Video, GetListingQueryResponseItem>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var mockStore = new Mock<IDataStore>();
            mockStore.Setup(s => s.GetRepository<Video>()).Returns(mockRepo.Object);

            var handler = new GetListingQueryHandler(mockStore.Object);

            var query = new GetListingQuery(0, 10, "search", "Title")
            {
                OrderByDirection = "asc"
            };

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expectedResponse.RecordsTotal, result.RecordsTotal);
            Assert.Equal(expectedResponse.Data.Count(), result.Data.Count());
            Assert.All(result.Data, item => Assert.True(item.Active));
        }

        [Fact]
        public async Task Handle_PropagatesException_WhenRepositoryThrows()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Video>>();
            mockRepo.Setup(r => r.GetDataTableResponseAsync(
                    It.IsAny<VideoSearchFilterSpecification>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<System.Linq.Expressions.Expression<Func<Video, GetListingQueryResponseItem>>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            var mockStore = new Mock<IDataStore>();
            mockStore.Setup(s => s.GetRepository<Video>()).Returns(mockRepo.Object);

            var handler = new GetListingQueryHandler(mockStore.Object);

            var query = new GetListingQuery(0, 10, "search", "Title");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(query, CancellationToken.None));
        }
    }
}
