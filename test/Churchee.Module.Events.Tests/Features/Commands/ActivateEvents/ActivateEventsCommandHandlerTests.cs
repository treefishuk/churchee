using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Events.Features.Commands;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Events.Tests.Features.Commands.ActivateEvents
{
    public class ActivateEventsCommandHandlerTests
    {
        [Fact]
        public async Task ActivateEventsCommandHandler_Handle_ReturnsCommandResponse()
        {
            //arrange
            var applicationTenantId = Guid.NewGuid();
            var request = new ActivateEventsCommand(applicationTenantId);
            var cut = new ActivateEventsCommandHandler(GetDataStore());

            //act
            var result = await cut.Handle(request, default);

            //assert
            result.IsSuccess.Should().BeTrue();
        }


        private static IDataStore GetDataStore()
        {
            var mockApplicationTenantRepository = new Mock<IRepository<ApplicationTenant>>();
            mockApplicationTenantRepository.Setup(s => s.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(new ApplicationTenant(Guid.NewGuid(), "Test", 1));

            var mockPageRepository = new Mock<IRepository<Page>>();
            var mockPageTypeRepository = new Mock<IRepository<PageType>>();
            var mockViewTemplateRepository = new Mock<IRepository<ViewTemplate>>();

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(mockApplicationTenantRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<PageType>()).Returns(mockPageTypeRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<Page>()).Returns(mockPageRepository.Object);
            mockDataStore.Setup(s => s.GetRepository<ViewTemplate>()).Returns(mockViewTemplateRepository.Object);

            return mockDataStore.Object;
        }

    }
}
