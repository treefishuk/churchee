using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Features.Churches.Commands;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Churchee.Module.Tenancy.Tests.Features.Commands.AddChurch
{
    public class AddChurchCommandHandlerTests
    {

        [Fact]
        public async Task AddChurchCommandHandler_Handle_ReturnsSuccessCommandResponse()
        {
            //arrange
            string name = "Test Church";
            int charityNumber = 123456;
            var request = new AddChurchCommand(name, charityNumber);

            var inMemorySettings = new Dictionary<string, string?> {
                {"Tenants:Domain", "*.churchee.com"}
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var cut = new AddChurchCommandHandler(GetDataStore(), configuration);

            //act
            var result = await cut.Handle(request, default);

            //assert
            result.IsSuccess.Should().BeTrue();
        }


        private static IDataStore GetDataStore()
        {
            var mockApplicationTenantRepository = new Mock<IRepository<ApplicationTenant>>();

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(mockApplicationTenantRepository.Object);

            return mockDataStore.Object;
        }

    }
}
