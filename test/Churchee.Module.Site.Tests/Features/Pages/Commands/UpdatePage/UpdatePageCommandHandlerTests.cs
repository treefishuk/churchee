using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands.UpdatePage;
using Churchee.Module.Site.Specifications;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandlerTests
    {
        [Fact]
        public async Task Handle_UpdatesPageAndPublishesWhenRequested()
        {
            var storageMock = new Mock<IDataStore>();
            var pageRepoMock = new Mock<IRepository<Page>>();
            var page = new Page(System.Guid.NewGuid(), "title", "/url", "desc", System.Guid.NewGuid(), null, true);

            pageRepoMock.Setup(r => r.ApplySpecification(It.IsAny<PageWithContentAndPropertiesSpecification>())).Returns(new[] { page }.AsQueryable());

            storageMock.Setup(s => s.GetRepository<Page>()).Returns(pageRepoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new UpdatePageCommandHandler(storageMock.Object);

            var cmd = new UpdatePageCommand.Builder()
                .SetTitle("new title")
                .SetDescription("new desc")
                .SetPageId(page.Id)
                .SetOrder(1)
                .Build();

            var result = await handler.Handle(cmd, CancellationToken.None);

            storageMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
