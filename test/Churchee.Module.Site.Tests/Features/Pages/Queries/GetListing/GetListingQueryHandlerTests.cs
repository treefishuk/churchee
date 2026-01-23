using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Queries;
using Churchee.Module.Site.Specifications;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.Features.Pages.Queries.GetListing
{
    public class GetListingQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsListingItems()
        {
            // Arrange
            var dataStore = GetDataStore();

            var handler = new GetListingQueryHandler(dataStore);

            var query = new GetListingQuery(1, 10, string.Empty, "Url desc", null);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.All(result.Data, item => Assert.Contains("Title", item.Title));
        }

        private static IDataStore GetDataStore()
        {
            var mockPageRepository = new Mock<IRepository<Page>>();

            mockPageRepository.Setup(s => s.GetDataTableResponseAsync(It.IsAny<PageListingSpecification>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Page, GetListingQueryResponseItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DataTableResponse<GetListingQueryResponseItem>()
                {
                    Data =
                    [
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Title", Url = "/url1" },
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Title 2", Url = "/url2" }
                    ]
                });

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Page>()).Returns(mockPageRepository.Object);
            return mockDataStore.Object;
        }

    }
}
