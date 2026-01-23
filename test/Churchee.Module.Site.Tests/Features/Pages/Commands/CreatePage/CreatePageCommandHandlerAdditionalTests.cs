using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandHandlerAdditionalTests
    {
        [Fact]
        public async Task Handle_WithNullParentId_GeneratesUrlWithoutParent()
        {
            var storageMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var repoMock = new Mock<IRepository<Page>>();

            var tenantId = System.Guid.NewGuid();
            currentUserMock.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);
            storageMock.Setup(s => s.GetRepository<Page>()).Returns(repoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreatePageCommandHandler(storageMock.Object, currentUserMock.Object);

            var cmd = new CreatePageCommand(
                title: "Test Page",
                description: "desc",
                pageTypeId: System.Guid.NewGuid().ToString(),
                parentId: null
            );

            var result = await handler.Handle(cmd, CancellationToken.None);

            repoMock.Verify(r => r.Create(It.IsAny<Page>()), Times.Once);
            storageMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
