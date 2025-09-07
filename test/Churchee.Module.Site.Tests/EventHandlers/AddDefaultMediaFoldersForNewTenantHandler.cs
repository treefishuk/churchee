using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Tenancy.Events;
using Moq;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddDefaultMediaFoldersForNewTenantHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<MediaFolder>> _MediaFolderRepositoryMock;
        private readonly AddDefaultMediaFoldersForNewTenantHandler _handler;

        public AddDefaultMediaFoldersForNewTenantHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _MediaFolderRepositoryMock = new Mock<IRepository<MediaFolder>>();
            _dataStoreMock.Setup(ds => ds.GetRepository<MediaFolder>()).Returns(_MediaFolderRepositoryMock.Object);
            _handler = new AddDefaultMediaFoldersForNewTenantHandler(_dataStoreMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateDefaultFolders()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();
            var tenantAddedEvent = new TenantAddedEvent(tenantId);

            _MediaFolderRepositoryMock.Setup(repo => repo.Create(It.IsAny<MediaFolder>()));
            _dataStoreMock.Setup(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _handler.Handle(tenantAddedEvent, CancellationToken.None);

            // Assert
            _MediaFolderRepositoryMock.Verify(repo => repo.Create(It.Is<MediaFolder>(p => p.ApplicationTenantId == tenantId && p.Name == "Images")), Times.Once);
            _MediaFolderRepositoryMock.Verify(repo => repo.Create(It.Is<MediaFolder>(p => p.ApplicationTenantId == tenantId && p.Name == "Fonts")), Times.Once);
            _dataStoreMock.Verify(ds => ds.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(1));
        }
    }
}
