using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Site.Events;
using Churchee.Module.Site.Specifications;
using Moq;
using Page = Churchee.Module.Site.Entities.Page;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class AddNewPageContentTypesToExistingPagesTests
    {
        [Fact]
        public async Task Handle_ShouldAddContentToExistingPages()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockPageRepository = new Mock<IRepository<Page>>();

            var pageTypeId = Guid.NewGuid();
            var pageTypeContentId = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();

            var pageTypeContentCreatedEvent = new PageTypeContentCreatedEvent(applicationTenantId, pageTypeContentId, pageTypeId);

            var pages = new List<Page>() {

                new Page(applicationTenantId, "Title", "/somewhere", "description", pageTypeId, null, false)
            };

            mockPageRepository
                .Setup(repo => repo.GetListAsync(It.IsAny<PagesWithContentThatImplementPageTypeSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pages);

            mockDataStore
                .Setup(store => store.GetRepository<Page>())
                .Returns(mockPageRepository.Object);

            var handler = new AddNewPageContentTypesToExistingPages(mockDataStore.Object);

            // Act
            await handler.Handle(pageTypeContentCreatedEvent, CancellationToken.None);

            // Assert
            mockDataStore.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotAddContent_WhenPageCountIsZero()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockPageRepository = new Mock<IRepository<Page>>();

            var pageTypeId = Guid.NewGuid();
            var pageTypeContentId = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();

            var pageTypeContentCreatedEvent = new PageTypeContentCreatedEvent(applicationTenantId, pageTypeContentId, pageTypeId);

            var pages = new List<Page>();

            mockPageRepository
                .Setup(repo => repo.GetListAsync(It.IsAny<PagesWithContentThatImplementPageTypeSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pages);

            mockDataStore
                .Setup(store => store.GetRepository<Page>())
                .Returns(mockPageRepository.Object);

            var handler = new AddNewPageContentTypesToExistingPages(mockDataStore.Object);

            // Act
            await handler.Handle(pageTypeContentCreatedEvent, CancellationToken.None);

            // Assert
            mockDataStore.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldNotAddContent_WhenNoPagesExist()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockPageRepository = new Mock<IRepository<Page>>();

            var pageTypeId = Guid.NewGuid();
            var pageTypeContentId = Guid.NewGuid();
            var applicationTenantId = Guid.NewGuid();

            var pageTypeContentCreatedEvent = new PageTypeContentCreatedEvent(applicationTenantId, pageTypeContentId, pageTypeId);

            var pages = null as List<Page>;

            mockPageRepository
                .Setup(repo => repo.GetListAsync(It.IsAny<PagesWithContentThatImplementPageTypeSpecification>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(pages);

            mockDataStore
                .Setup(store => store.GetRepository<Page>())
                .Returns(mockPageRepository.Object);

            var handler = new AddNewPageContentTypesToExistingPages(mockDataStore.Object);

            // Act
            await handler.Handle(pageTypeContentCreatedEvent, CancellationToken.None);

            // Assert
            mockDataStore.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }

}