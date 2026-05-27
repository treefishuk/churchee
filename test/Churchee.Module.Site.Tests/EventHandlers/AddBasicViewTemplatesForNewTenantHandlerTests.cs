using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Events;
using Moq;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddBasicViewTemplatesForNewTenantHandlerTests
    {

        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<ViewTemplate>> _mockViewTemplateMock;
        private readonly Mock<IRepository<ApplicationTenant>> _mockApplicationTenantMock;

        public AddBasicViewTemplatesForNewTenantHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();

            _mockViewTemplateMock = new Mock<IRepository<ViewTemplate>>();
            _mockApplicationTenantMock = new Mock<IRepository<ApplicationTenant>>();

            _dataStoreMock
                .Setup(store => store.GetRepository<ViewTemplate>())
                .Returns(_mockViewTemplateMock.Object);

            _dataStoreMock
                .Setup(store => store.GetRepository<ApplicationTenant>())
                .Returns(_mockApplicationTenantMock.Object);
        }


        [Fact]
        public async Task Handle_Should_Add_Default_Templates()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            var tenantAddedEvent = new TenantAddedEvent(tenantId);

            var response = new ApplicationTenant(tenantId, "Test Tenant", 0);

            _mockApplicationTenantMock
                .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var handler = new AddBasicViewTemplatesForNewTenantHandler(_dataStoreMock.Object);

            // Act
            await handler.Handle(tenantAddedEvent, CancellationToken.None);

            // Assert
            _mockViewTemplateMock.Verify(repo => repo.Create(It.Is<ViewTemplate>(pc => (pc.ApplicationTenantId == tenantId) && pc.Location == "/Views/Shared/_Layout.cshtml")), Times.Exactly(1));
            _mockViewTemplateMock.Verify(repo => repo.Create(It.Is<ViewTemplate>(pc => (pc.ApplicationTenantId == tenantId) && pc.Location == "/Views/Shared/_ViewImports.cshtml")), Times.Exactly(1));
            _mockViewTemplateMock.Verify(repo => repo.Create(It.Is<ViewTemplate>(pc => (pc.ApplicationTenantId == tenantId) && pc.Location == "/Views/Shared/_ViewStart.cshtml")), Times.Exactly(1));
            _dataStoreMock.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}