using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Tenancy.Events;
using Moq;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddStylesForNewTenantHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<Css>> _cssRepositoryMock;
        private readonly AddStylesForNewTenantHandler _handler;

        public AddStylesForNewTenantHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _cssRepositoryMock = new Mock<IRepository<Css>>();
            _dataStoreMock.Setup(ds => ds.GetRepository<Css>()).Returns(_cssRepositoryMock.Object);
            _handler = new AddStylesForNewTenantHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateDefaultStyles()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();
            var tenantAddedEvent = new TenantAddedEvent(tenantId);

            _cssRepositoryMock.Setup(repo => repo.Create(It.IsAny<Css>()));
            _dataStoreMock.Setup(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _handler.Handle(tenantAddedEvent, CancellationToken.None);

            // Assert
            _cssRepositoryMock.Verify(repo => repo.Create(It.Is<Css>(p => p.ApplicationTenantId == tenantId)), Times.Once);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}
