using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using Moq;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddPageContentTypesToNewPageTests
    {

        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<IRepository<PageType>> _mockPageTypeRepositoryMock;
        private readonly Mock<IRepository<PageContent>> _mockPageContentRepository;

        public AddPageContentTypesToNewPageTests()
        {
            _dataStoreMock = new Mock<IDataStore>();

            _mockPageTypeRepositoryMock = new Mock<IRepository<PageType>>();
            _mockPageContentRepository = new Mock<IRepository<PageContent>>();

            _dataStoreMock
                .Setup(store => store.GetRepository<PageType>())
                .Returns(_mockPageTypeRepositoryMock.Object);

            _dataStoreMock
                .Setup(store => store.GetRepository<PageContent>())
                .Returns(_mockPageContentRepository.Object);
        }


        [Fact]
        public async Task Handle_ShouldAddTwoPageContentTypesToNewPage_WhenTwoContentTypes()
        {
            // Arrange
            var pageTypeId = Guid.NewGuid();
            var pageId = Guid.NewGuid();
            var pageCreatedEvent = new PageCreatedEvent(pageId, pageTypeId);

            var pageType = new PageType(pageTypeId, Guid.NewGuid(), Guid.NewGuid(), false, "Test");
            pageType.AddPageTypeContent(Guid.NewGuid(), "Test", "type", false, 1);
            pageType.AddPageTypeContent(Guid.NewGuid(), "Test2 ", "type", false, 1);

            _mockPageTypeRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<PageTypeWithPageTypeContentSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pageType);


            var handler = new AddPageContentTypesToNewPage(_dataStoreMock.Object);

            // Act
            await handler.Handle(pageCreatedEvent, CancellationToken.None);

            // Assert
            _mockPageContentRepository.Verify(repo => repo.Create(It.Is<PageContent>(pc => pc.PageId == pageId)), Times.Exactly(2));
            _dataStoreMock.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldAddOnePageContentTypesToNewPage_WhenOneContentTypes()
        {
            // Arrange
            var pageTypeId = Guid.NewGuid();
            var pageId = Guid.NewGuid();
            var pageCreatedEvent = new PageCreatedEvent(pageId, pageTypeId);

            var pageType = new PageType(pageTypeId, Guid.NewGuid(), Guid.NewGuid(), false, "Test");
            pageType.AddPageTypeContent(Guid.NewGuid(), "Test", "type", false, 1);

            _mockPageTypeRepositoryMock
                .Setup(repo => repo.FirstOrDefaultAsync(It.IsAny<PageTypeWithPageTypeContentSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pageType);

            var handler = new AddPageContentTypesToNewPage(_dataStoreMock.Object);

            // Act
            await handler.Handle(pageCreatedEvent, CancellationToken.None);

            // Assert
            _mockPageContentRepository.Verify(repo => repo.Create(It.Is<PageContent>(pc => pc.PageId == pageId)), Times.Once);
            _dataStoreMock.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}