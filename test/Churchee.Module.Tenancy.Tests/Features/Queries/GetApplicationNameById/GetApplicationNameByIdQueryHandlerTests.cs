using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Tenancy.Features.Churches.Queries;
using Churchee.Module.Tenancy.Specifications;
using Churchee.Test.Helpers.Validation;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Tenancy.Tests.Features.Queries.GetApplicationNameById
{
    public class GetApplicationNameByIdQueryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Return_Application_Name()
        {
            // Arrange
            var applicationId = Guid.NewGuid();
            var query = new GetApplicationNameByIdQuery(applicationId);

            // Act
            var queryHandler = new GetApplicationNameByIdQueryHandler(GetDataStore());

            string response = await queryHandler.Handle(query, default);

            // Assert
            response.Should().NotBeNull();
            response.Should().NotBeEmpty();
        }

        private static IDataStore GetDataStore()
        {
            var mockApplicationTenantRepository = new Mock<IRepository<ApplicationTenant>>();

            mockApplicationTenantRepository.Setup(s => s.FirstOrDefaultAsync(It.IsAny<ApplicationTenantByIdSpecification>(), It.IsAny<Expression<Func<ApplicationTenant, string>>>(), It.IsAny<CancellationToken>())).ReturnsAsync("tenantName");

            var mockDataStore = new Mock<IDataStore>();

            mockDataStore.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(mockApplicationTenantRepository.Object);

            return mockDataStore.Object;
        }
    }
}
