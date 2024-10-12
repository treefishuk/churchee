using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Entities;
using Churchee.Module.Podcasts.Features.Queries;
using Churchee.Module.Podcasts.Specifications;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Podcasts.Tests.Features.Queries.GetListing
{
    public class GetListingQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<Podcast>> _repositoryMock;

        public GetListingQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _repositoryMock = new Mock<IRepository<Podcast>>();
            _repositoryMock.Setup(s => s.GetDataTableResponseAsync(It.IsAny<PodcastSearchFilterSpecification>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Podcast, GetListingQueryResponseItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DataTableResponse<GetListingQueryResponseItem>()
                {
                    Data =
                    [
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Test", Source = "Test", Active = true },
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Test 2", Source = "Test", Active = true}
                    ]
                });


            _dataStoreMock.Setup(ds => ds.GetRepository<Podcast>()).Returns(_repositoryMock.Object);

        }

        [Fact]
        public async Task Handle_ShouldReturnPodcasts()
        {
            // Arrange
            var query = new GetListingQuery(0, 10, string.Empty, "Title asc");
            var handler = new GetListingQueryHandler(_dataStoreMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Count().Should().BeGreaterThan(0);
        }
    }
}
