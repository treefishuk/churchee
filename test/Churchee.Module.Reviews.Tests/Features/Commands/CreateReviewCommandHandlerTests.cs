using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Reviews.Entities;
using Churchee.Module.Reviews.Features.Commands;
using Moq;

namespace Churchee.Module.Reviews.Tests.Features.Commands
{
    public class CreateReviewCommandHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<ICurrentUser> _currentUserMock;

        public CreateReviewCommandHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _currentUserMock = new Mock<ICurrentUser>();
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Review>>();

            _dataStoreMock.Setup(x => x.GetRepository<Review>()).Returns(mockRepo.Object);

            var command = new CreateReviewCommand(5, "Great review!", "John Doe");

            _currentUserMock.Setup(cu => cu.GetApplicationTenantId()).ReturnsAsync(Guid.NewGuid());

            var handler = new CreateReviewCommandHandler(_dataStoreMock.Object, _currentUserMock.Object);

            // Act
            var result = await handler.Handle(command, default);

            // Assert
            Assert.True(result.IsSuccess);
        }


    }
}
