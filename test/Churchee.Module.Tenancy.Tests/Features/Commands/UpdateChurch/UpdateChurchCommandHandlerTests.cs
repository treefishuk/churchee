using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Features.Churches.Commands;
using Churchee.Module.Tenancy.Features.Churches.Commands.UpdateChurch;
using Churchee.Module.Tenancy.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Tenancy.Tests.Features.Commands.UpdateChurch
{
    public class UpdateChurchCommandHandlerTests
    {

        private readonly Mock<IRepository<ApplicationHost>> _mockHostsRepo;
        private readonly Mock<IRepository<ApplicationTenant>> _mockAppTenantRepo;

        public UpdateChurchCommandHandlerTests()
        {
            _mockHostsRepo = new Mock<IRepository<ApplicationHost>>();
            _mockAppTenantRepo = new Mock<IRepository<ApplicationTenant>>();
        }

        [Fact]
        public async Task UpdateChurchCommandHandler_Handle_ReturnsCommandResponse()
        {
            // Arrange
            var churchId = Guid.NewGuid();
            int newCharityNumber = 654321;
            var domains = new List<string> { "updatedchurch.churchee.com" };
            var request = new UpdateChurchCommand(churchId, newCharityNumber, domains);

            _mockHostsRepo.Setup(s => s.GetListAsync(It.IsAny<ApplicationHostsByApplicationTenantIdSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            _mockAppTenantRepo.Setup(s => s.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationTenant(Guid.NewGuid(), "Test", 1));

            var cut = new UpdateChurchCommandHandler(GetDataStore());

            // Act
            var result = await cut.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_DoesNotCreateDuplicateHost_When_DomainAlreadyExists()
        {
            // Arrange
            var churchId = Guid.NewGuid();
            var domains = new List<string> { "existingchurch.churchee.com" };
            var request = new UpdateChurchCommand(churchId, 123, domains);

            var existingHost = new ApplicationHost("existingchurch.churchee.com", churchId);
            var existingHosts = new List<ApplicationHost> { existingHost };

            _mockAppTenantRepo.Setup(s => s.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationTenant(churchId, "Test", 1));

            _mockHostsRepo.Setup(s => s.GetListAsync(It.IsAny<ApplicationHostsByApplicationTenantIdSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(existingHosts);

            // Track Create calls
            _mockHostsRepo.Setup(s => s.Create(It.IsAny<ApplicationHost>())).Verifiable();

            var handler = new UpdateChurchCommandHandler(GetDataStore());

            // Act
            var response = await handler.Handle(request, CancellationToken.None);

            // Assert
            response.IsSuccess.Should().BeTrue();
            _mockHostsRepo.Verify(s => s.Create(It.IsAny<ApplicationHost>()), Times.Never, "When the domain already exists we should not create a duplicate host entry.");
        }

        private IDataStore GetDataStore()
        {
            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(_mockAppTenantRepo.Object);
            mockDataStore.Setup(s => s.GetRepository<ApplicationHost>()).Returns(_mockHostsRepo.Object);

            return mockDataStore.Object;
        }

    }
}
