using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.PageTypes.Commands.CreatePageTypeContent;
using Churchee.Module.Site.Specifications;
using Moq;

namespace Churchee.Module.Site.Tests.Features.PageTypes.Commands.CreatePageTypeContent
{
    public class CreatePageTypeContentAdditionalTests
    {
        [Fact]
        public async Task Handle_AddsContent_WhenPageTypeFound()
        {
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<PageType>>();
            var pageType = new PageType(System.Guid.NewGuid(), System.Guid.NewGuid(), System.Guid.NewGuid(), true, "Test");

            repoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<GetPageTypeByIdAndIncludePageTypeContentSpecification>(), It.IsAny<CancellationToken>())).ReturnsAsync(pageType);
            storageMock.Setup(s => s.GetRepository<PageType>()).Returns(repoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new CreatePageTypeContentCommandHandler(storageMock.Object);
            var cmd = new CreatePageTypeContentCommand(System.Guid.NewGuid(), "Name", "type", false, 1);

            var result = await handler.Handle(cmd, CancellationToken.None);

            storageMock.Verify(s => s.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
