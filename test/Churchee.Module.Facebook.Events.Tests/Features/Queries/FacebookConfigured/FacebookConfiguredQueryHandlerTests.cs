using Ardalis.Specification;
using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Facebook.Events.Features.Queries;
using Churchee.Module.Tokens.Entities;
using Moq;

namespace Churchee.Module.Facebook.Events.Tests.Features.Queries
{
    public class FacebookConfiguredQueryHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock = new();
        private readonly Mock<ICurrentUser> _currentUserMock = new();
        private readonly Mock<IRepository<Token>> _tokenRepoMock = new();

        private FacebookConfiguredQueryHandler CreateHandler()
        {
            _dataStoreMock.Setup(ds => ds.GetRepository<Token>()).Returns(_tokenRepoMock.Object);
            return new FacebookConfiguredQueryHandler(_dataStoreMock.Object, _currentUserMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsTrue_WhenAccessTokenExists()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _currentUserMock.Setup(u => u.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var tokens = new[]
            {
                new Token(tenantId, "FacebookAccessToken", "token-value")
            }.AsQueryable();

            _tokenRepoMock
                .Setup(r => r.ApplySpecification(It.IsAny<ISpecification<Token>>(), false))
                .Returns(tokens);

            var handler = CreateHandler();
            var query = new FacebookConfiguredQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenAccessTokenIsNullOrEmpty()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _currentUserMock.Setup(u => u.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var tokens = new[]
            {
                new Token(tenantId, "FacebookAccessToken", string.Empty)
            }.AsQueryable();

            _tokenRepoMock
                .Setup(r => r.ApplySpecification(It.IsAny<ISpecification<Token>>(), false))
                .Returns(tokens);

            var handler = CreateHandler();
            var query = new FacebookConfiguredQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenNoTokenFound()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            _currentUserMock.Setup(u => u.GetApplicationTenantId()).ReturnsAsync(tenantId);

            var tokens = Enumerable.Empty<Token>().AsQueryable();

            _tokenRepoMock
                .Setup(r => r.ApplySpecification(It.IsAny<ISpecification<Token>>(), false))
                .Returns(tokens);

            var handler = CreateHandler();
            var query = new FacebookConfiguredQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result);
        }
    }
}
