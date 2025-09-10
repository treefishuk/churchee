using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Videos.Entities;
using Churchee.Module.Videos.Features.Queries;
using Churchee.Module.Videos.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Videos.Tests.Features.Queries.GetListing
{
    public class GetListingQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsExpectedDataTableResponse()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Video>>();
            var mockDataStore = new Mock<IDataStore>();

            var expectedItems = new List<GetListingQueryResponseItem>
            {
                new() {
                    Id = Guid.NewGuid(),
                    Active = true,
                    ImageUri = "/img/1.png",
                    Title = "Test Video",
                    PublishedDate = DateTime.UtcNow,
                    Source = "YouTube"
                }
            };

            var expectedResponse = new DataTableResponse<GetListingQueryResponseItem>
            {
                Draw = 1,
                RecordsTotal = 1,
                RecordsFiltered = 1,
                Data = expectedItems,
                Error = null
            };

            mockRepo
                .Setup(r => r.GetDataTableResponseAsync(
                    It.IsAny<VideoSearchFilterSpecification>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Video, GetListingQueryResponseItem>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            mockDataStore.Setup(ds => ds.GetRepository<Video>()).Returns(mockRepo.Object);

            var handler = new GetListingQueryHandler(mockDataStore.Object);

            var query = new GetListingQuery(0, 10, "Test", "Title asc");

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeEquivalentTo(expectedItems);
            result.RecordsTotal.Should().Be(1);
            result.RecordsFiltered.Should().Be(1);
            result.Error.Should().BeNull();

            mockRepo.Verify(r => r.GetDataTableResponseAsync(
                It.IsAny<VideoSearchFilterSpecification>(),
                "Title",
                "asc",
                0,
                10,
                It.IsAny<Expression<Func<Video, GetListingQueryResponseItem>>>(),
                It.IsAny<CancellationToken>()), Times.Once);

            mockDataStore.Verify(ds => ds.GetRepository<Video>(), Times.Once);
        }

        [Fact]
        public async Task Handle_PropagatesRepositoryException()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Video>>();
            var mockDataStore = new Mock<IDataStore>();

            mockRepo
                .Setup(r => r.GetDataTableResponseAsync(
                    It.IsAny<VideoSearchFilterSpecification>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Expression<Func<Video, GetListingQueryResponseItem>>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Repository error"));

            mockDataStore.Setup(ds => ds.GetRepository<Video>()).Returns(mockRepo.Object);

            var handler = new GetListingQueryHandler(mockDataStore.Object);

            var query = new GetListingQuery(0, 10, string.Empty, string.Empty);

            // Act
            Func<Task> act = async () => await handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>("Repository error");
        }
    }
}