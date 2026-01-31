using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Jotform.Features.Queries.GetJotformConfigurationStatus;
using Churchee.Module.Tokens.Entities;
using Churchee.Module.Tokens.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Jotform.Tests.Features.Queries
{
    public class GetJotformConfigurationStatusQueryHandlerTests
    {

        [Fact]
        public async Task GetJotformConfigurationStatusQueryHandler_Handle_Returns_False_When_Not_Configured()
        {
            // Arrange
            var currentUserMock = new Mock<ICurrentUser>();

            var cut = new GetJotformConfigurationStatusQueryHandler(GetDataStore(false), currentUserMock.Object);

            // Act
            var response = await cut.Handle(new GetJotformConfigurationStatusQuery(), CancellationToken.None);

            // Assert
            response.Configured.Should().BeFalse();
        }

        [Fact]
        public async Task GetJotformConfigurationStatusQueryHandler_Handle_Returns_True_When_Configured()
        {
            // Arrange
            var currentUserMock = new Mock<ICurrentUser>();

            var cut = new GetJotformConfigurationStatusQueryHandler(GetDataStore(true), currentUserMock.Object);

            // Act
            var response = await cut.Handle(new GetJotformConfigurationStatusQuery(), CancellationToken.None);

            // Assert
            response.Configured.Should().BeTrue();

        }

        private static IDataStore GetDataStore(bool hasToken)
        {
            var mockTokenRepository = new Mock<IRepository<Token>>();

            if (hasToken)
            {
                var setup = mockTokenRepository.Setup(s => s.FirstOrDefaultAsync(
                It.IsAny<GetTokenByKeySpecification>(),
                It.IsAny<CancellationToken>()))
                  .ReturnsAsync(new Token(Guid.NewGuid(), "sss", ""));
            }

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<Token>()).Returns(mockTokenRepository.Object);

            return mockDataStore.Object;
        }
    }
}
