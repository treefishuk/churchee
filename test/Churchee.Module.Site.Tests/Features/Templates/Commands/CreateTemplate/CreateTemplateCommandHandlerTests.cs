using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Templates.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Templates.Commands.CreateTemplate
{
    public class CreateTemplateCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesTemplateAndSavesChanges()
        {
            var storageMock = new Mock<IDataStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var repoMock = new Mock<IRepository<ViewTemplate>>();

            var tenantId = System.Guid.NewGuid();
            currentUserMock.Setup(c => c.GetApplicationTenantId()).ReturnsAsync(tenantId);
            storageMock.Setup(s => s.GetRepository<ViewTemplate>()).Returns(repoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreateTemplateCommandHandler(storageMock.Object, currentUserMock.Object);
            var cmd = new CreateTemplateCommand("/path", "content");

            var result = await handler.Handle(cmd, CancellationToken.None);

            repoMock.Verify(r => r.Create(It.IsAny<ViewTemplate>()), Times.Once);
            storageMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
