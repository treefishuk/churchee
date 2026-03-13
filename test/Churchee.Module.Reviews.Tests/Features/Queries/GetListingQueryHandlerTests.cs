using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Reviews.Entities;
using Churchee.Module.Reviews.Features.Queries;
using Churchee.Module.Reviews.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Reviews.Tests.Features.Queries.GetListing
{
    public class GetListingQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<Review>> _repositoryMock;

        public GetListingQueryHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _repositoryMock = new Mock<IRepository<Review>>();
            _repositoryMock.Setup(s => s.GetDataTableResponseAsync(It.IsAny<ReviewTextFilterSpecification>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Review, GetListingQueryResponseItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DataTableResponse<GetListingQueryResponseItem>()
                {
                    Data =
                    [
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Reviewer = "Test", Rating = 3, CreatedDate = DateTime.Now.AddDays(-3)},
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Reviewer = "Test 2", Rating = 4, CreatedDate = DateTime.Now.AddDays(-3)}
                    ]
                });


            _dataStoreMock.Setup(ds => ds.GetRepository<Review>()).Returns(_repositoryMock.Object);

        }

        [Fact]
        public async Task Handle_ShouldReturnPodcasts()
        {
            // Arrange
            var query = new GetListingQuery(0, 10, string.Empty, "CreatedDate asc");
            var handler = new GetListingQueryHandler(_dataStoreMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Data.Count().Should().BeGreaterThan(0);
        }
    }
}
