using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Tenancy.Events;
using Moq;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddDefaultPagesForNewTenantHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<Page>> _pageRepositoryMock;
        private readonly Mock<IRepository<PageType>> _pageTypeRepositoryMock;
        private readonly AddDefaultPagesForNewTenantHandler _handler;

        public AddDefaultPagesForNewTenantHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _pageRepositoryMock = new Mock<IRepository<Page>>();
            _pageTypeRepositoryMock = new Mock<IRepository<PageType>>();

            _dataStoreMock.Setup(ds => ds.GetRepository<Page>()).Returns(_pageRepositoryMock.Object);
            _dataStoreMock.Setup(ds => ds.GetRepository<PageType>()).Returns(_pageTypeRepositoryMock.Object);

            _handler = new AddDefaultPagesForNewTenantHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateHomePageAndPageType()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();
            var tenantAddedEvent = new TenantAddedEvent(tenantId);

            _pageTypeRepositoryMock.Setup(repo => repo.Create(It.IsAny<PageType>()));
            _pageRepositoryMock.Setup(repo => repo.Create(It.IsAny<Page>()));
            _dataStoreMock.Setup(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _handler.Handle(tenantAddedEvent, CancellationToken.None);

            // Assert
            _pageTypeRepositoryMock.Verify(repo => repo.Create(It.Is<PageType>(pt => pt.ApplicationTenantId == tenantId && pt.Name == "Home")), Times.Once);
            _pageRepositoryMock.Verify(repo => repo.Create(It.Is<Page>(p => p.ApplicationTenantId == tenantId && p.Title == "Home")), Times.Once);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
