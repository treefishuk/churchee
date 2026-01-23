using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Templates.Commands.UpdateTemplateContent;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Templates.Commands.UpdateTemplateContent
{
    public class UpdateTemplateContentCommandHandlerTests
    {
        [Fact]
        public async Task Handle_UpdatesTemplateContentAndSaves()
        {
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<ViewTemplate>>();
            var template = new ViewTemplate(System.Guid.NewGuid(), "/path", "old");

            repoMock.Setup(r => r.GetByIdAsync(It.IsAny<object>(), It.IsAny<CancellationToken>())).ReturnsAsync(template);
            storageMock.Setup(s => s.GetRepository<ViewTemplate>()).Returns(repoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new UpdateTemplateContentCommandHandler(storageMock.Object);
            var cmd = new UpdateTemplateContentComand(template.Id, "new content");

            var result = await handler.Handle(cmd, CancellationToken.None);

            repoMock.Verify(r => r.Update(It.IsAny<ViewTemplate>()), Times.Once);
            storageMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
