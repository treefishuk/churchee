using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Jotform.Features.Commands;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Jotform.Tests.Features.Commands
{
    public class ConfigureJotformCommandHandlerTests
    {

        [Fact]
        public async Task ConfigureJotformCommandHandler_Returns_CommandResponse()
        {
            // Arrange
            var mockDataStore = GetDataStore();
            var mockCurrentUser = new Mock<ICurrentUser>();
            var commandHandler = new ConfigureJotformCommandHandler(mockDataStore, mockCurrentUser.Object);
            var command = new ConfigureJotformCommand("test-api-key");
            var cancellationToken = new CancellationToken();

            // Act
            var result = await commandHandler.Handle(command, cancellationToken);

            // Assert
            result.Should().BeOfType<CommandResponse>();
        }

        private static IDataStore GetDataStore()
        {
            var mockTokenRepository = new Mock<IRepository<Token>>();

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<Token>()).Returns(mockTokenRepository.Object);

            return mockDataStore.Object;
        }
    }
}
