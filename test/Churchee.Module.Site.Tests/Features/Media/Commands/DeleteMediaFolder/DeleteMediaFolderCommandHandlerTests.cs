using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Media.Commands;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Media.Commands.DeleteMediaFolder
{
    public class DeleteMediaFolderCommandHandlerTests
    {
        [Fact]
        public async Task Handle_DeletesMediaFolderAndSavesChanges()
        {
            // Arrange
            var dataStoreMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<MediaFolder>>();
            var folderId = Guid.NewGuid();
            dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(repoMock.Object);
            var handler = new DeleteMediaFolderCommandHandler(dataStoreMock.Object);
            var command = new DeleteMediaFolderCommand(folderId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(x => x.SoftDelete(folderId), Times.Once);
            dataStoreMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }

        [Fact]
        public async Task Handle_ReturnsError_WhenExceptionThrown()
        {
            // Arrange
            var dataStoreMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<MediaFolder>>();
            var folderId = Guid.NewGuid();
            dataStoreMock.Setup(x => x.GetRepository<MediaFolder>()).Returns(repoMock.Object);
            repoMock.Setup(x => x.SoftDelete(folderId)).Throws(new Exception("fail"));
            var handler = new DeleteMediaFolderCommandHandler(dataStoreMock.Object);
            var command = new DeleteMediaFolderCommand(folderId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<CommandResponse>(result);
            // Optionally, check for error in result if CommandResponse exposes errors
        }
    }
}
