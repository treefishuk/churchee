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
        [Fact]
        public async Task UpdateChurchCommandHandler_Handle_ReturnsCommandResponse()
        {
            // Arrange
            var churchId = Guid.NewGuid();
            int newCharityNumber = 654321;
            var domains = new List<string> { "updatedchurch.churchee.com" };
            var request = new UpdateChurchCommand(churchId, newCharityNumber, domains);
            var cut = new UpdateChurchCommandHandler(GetDataStore());

            // Act
            var result = await cut.Handle(request, default);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        private static IDataStore GetDataStore()
        {
            var mockApplicationTenantRepository = new Mock<IRepository<ApplicationTenant>>();
            mockApplicationTenantRepository.Setup(s => s.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ApplicationTenant(Guid.NewGuid(), "Test", 1));

            var mockHostsRepository = new Mock<IRepository<ApplicationHost>>();

            mockHostsRepository.Setup(s => s.GetListAsync(It.IsAny<ApplicationHostsByApplicationIdSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync([]);

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(mockApplicationTenantRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<ApplicationHost>()).Returns(mockHostsRepository.Object);

            return mockDataStore.Object;
        }

    }
}
