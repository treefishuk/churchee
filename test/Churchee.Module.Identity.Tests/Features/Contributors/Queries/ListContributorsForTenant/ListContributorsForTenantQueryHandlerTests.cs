using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Identity.Entities;
using Churchee.Module.Identity.Features.Contributors.Queries;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Identity.Tests.Features.Contributors.Queries.ListContributorsForTenant
{
    public class ListContributorsForTenantQueryHandlerTests
    {
        [Fact]
        public async Task ListContributorsForTenantQueryHandler_Handle()
        {
            //arrange
            var mockCurrentUser = new Mock<ICurrentUser>();

            mockCurrentUser.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(Guid.NewGuid());

            var cut = new ListContributorsForTenantQueryHandler(GetDataStore(), mockCurrentUser.Object);

            var query = new ListContributorsForTenantQuery();

            //act
            var data = await cut.Handle(query, default);

            data.Should().NotBeNull();

        }

        private static IDataStore GetDataStore()
        {
            var mockApplicationUserRepository = new Mock<IRepository<ApplicationUser>>();

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationUser>()).Returns(mockApplicationUserRepository.Object);

            return mockDataStore.Object;
        }
    }
}
