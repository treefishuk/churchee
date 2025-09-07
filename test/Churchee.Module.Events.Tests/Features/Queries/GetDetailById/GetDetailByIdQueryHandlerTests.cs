using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Queries;
using Churchee.Module.Events.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Queries.GetDetailById
{
    public class GetDetailByIdQueryHandlerTests
    {
        [Fact]
        public async Task GetDetailByIdQueryHandler_Handle_ReturnsGetDetailByIdResponse()
        {

            //arrange
            var testEvent = new Event.Builder()
                .SetApplicationTenantId(Guid.NewGuid())
                .SetParentId(Guid.NewGuid())
                .SetParentSlug("/events")
                .SetTitle("Test Event")
                .SetPageTypeId(Guid.NewGuid())
                .SetDates(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(-1))
                .Build();

            var cut = new GetDetailByIdQueryHandler(GetDataStore(testEvent));

            //act
            var response = await cut.Handle(GetQuery(), default);

            //assert
            response.Should().BeOfType<GetDetailByIdResponse>();
        }

        private static IDataStore GetDataStore(Event testEvent)
        {
            var mockEventRepository = new Mock<IRepository<Event>>();

            mockEventRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<EventByIdIncludingDatesSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(testEvent);

            var mockEventDateRepository = new Mock<IRepository<EventDate>>();

            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(mockEventRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<EventDate>()).Returns(mockEventDateRepository.Object);
            return mockDataStore.Object;
        }

        private static GetDetailByIdQuery GetQuery()
        {
            return new GetDetailByIdQuery(Guid.NewGuid());
        }
    }
}
