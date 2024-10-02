using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Features.Commands;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Commands.DeleteEvent
{
    public class DeleteEventCommandHandlerTests
    {
        [Fact]
        public async Task DeleteEventCommand_Handle_Returns_CommandResponse()
        {
            //arrange
            var id = Guid.NewGuid();
            var command = new DeleteEventCommand(id);
            var cut = new DeleteEventCommandHandler(GetDataStore());

            //act
            var result = await cut.Handle(command, default);

            //assert
            result.IsSuccess.Should().BeTrue();
        }

        private static IDataStore GetDataStore()
        {
            var mockEventRepository = new Mock<IRepository<Event>>();
            var mockDataStore = new Mock<IDataStore>();
            mockDataStore.Setup(s => s.GetRepository<Event>()).Returns(mockEventRepository.Object);
            return mockDataStore.Object;
        }
    }
}
