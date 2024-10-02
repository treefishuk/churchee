using Churchee.Common.Abstractions;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.Events.Specifications;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Events.Tests.Features.Queries.GetListing
{
    public class GetListingQueryHandlerTests
    {
        [Fact]
        public async Task GetListingQueryHandler_Handler_ReturnsDataTableResponse()
        {
            //arrange
            var cut = new GetListingQueryHandler(GetDataStore());

            //act
            var response = await cut.Handle(GetQuery(), default);

            //assert
            response.Should().BeOfType<DataTableResponse<GetListingQueryResponseItem>>();
        }

        private static IDataStore GetDataStore()
        {
            var mockEventRepository = new Mock<IRepository<Event>>();

            mockEventRepository.Setup(s => s.GetDataTableResponseAsync(It.IsAny<EventTextFilterSpecification>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Event, GetListingQueryResponseItem>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new DataTableResponse<GetListingQueryResponseItem>()
                {
                    Data =
                    [
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Test", Source = "Test", Active = true, CreatedDate = DateTime.Now, NextDate = DateTime.Now.AddDays(3) },
                        new GetListingQueryResponseItem() { Id = Guid.NewGuid(), Title = "Test 2", Source = "Test", Active = true, CreatedDate = DateTime.Now, NextDate = DateTime.Now.AddDays(3) }
                    ]
                });

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(mockEventRepository.Object);
            return mockDataStore.Object;
        }

        private static GetListingQuery GetQuery()
        {
            var query = new GetListingQuery(0, 100, "test", "CreatedDate asc");

            return query;
        }

    }
}
