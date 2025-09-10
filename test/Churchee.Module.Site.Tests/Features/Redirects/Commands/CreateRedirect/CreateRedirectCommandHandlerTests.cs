using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Redirects.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Redirects.Commands.CreateRedirect
{
    public class CreateRedirectCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesRedirectAndSavesChanges()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<RedirectUrl>>();
            storageMock.Setup(x => x.GetRepository<RedirectUrl>()).Returns(repoMock.Object);
            var handler = new CreateRedirectCommandHandler(storageMock.Object);
            var command = new CreateRedirectCommand("/test", Guid.NewGuid());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(x => x.Create(It.IsAny<RedirectUrl>()), Times.Once);
            storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }

        [Fact]
        public async Task Handle_AddsLeadingSlash_IfMissing()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<RedirectUrl>>();
            storageMock.Setup(x => x.GetRepository<RedirectUrl>()).Returns(repoMock.Object);
            var handler = new CreateRedirectCommandHandler(storageMock.Object);
            var command = new CreateRedirectCommand("test", Guid.NewGuid());

            // Act
            await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.StartsWith("/", command.Path);
        }
    }
}
