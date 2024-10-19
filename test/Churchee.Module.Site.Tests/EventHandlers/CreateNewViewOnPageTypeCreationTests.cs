using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.EventHandlers;
using Churchee.Module.Site.Events;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.EventHandlers
{
    public class CreateNewViewOnPageTypeCreationTests
    {
        [Fact]
        public async Task Handle_ShouldCreateNewViewTemplate_WhenViewTemplateDoesNotExist()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockViewTemplateRepository = new Mock<IRepository<ViewTemplate>>();
            var applicationTenantId = Guid.NewGuid();

            var pageTypeCreatedEvent = new PageTypeCreatedEvent(applicationTenantId, "TestPageType");

            mockViewTemplateRepository
                .Setup(repo => repo.AnyWithFiltersDisabled(It.IsAny<Expression<Func<ViewTemplate, bool>>>()))
                .Returns(false);

            mockDataStore
                .Setup(store => store.GetRepository<ViewTemplate>())
                .Returns(mockViewTemplateRepository.Object);

            var handler = new CreateNewViewOnPageTypeCreation(mockDataStore.Object);

            // Act
            await handler.Handle(pageTypeCreatedEvent, CancellationToken.None);

            // Assert
            mockViewTemplateRepository.Verify(repo => repo.Create(It.IsAny<ViewTemplate>()), Times.Once);
            mockDataStore.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldNotCreateNewViewTemplate_WhenViewTemplateAlreadyExists()
        {
            // Arrange
            var mockDataStore = new Mock<IDataStore>();
            var mockViewTemplateRepository = new Mock<IRepository<ViewTemplate>>();
            var applicationTenantId = Guid.NewGuid();

            var pageTypeCreatedEvent = new PageTypeCreatedEvent(applicationTenantId, "TestPageType");

            var existingViewTemplate = new ViewTemplate(applicationTenantId, "/Views/Shared/TestPageType.cshtml", "<div></div>");

            mockViewTemplateRepository
                .Setup(repo => repo.AnyWithFiltersDisabled(It.IsAny<Expression<Func<ViewTemplate, bool>>>()))
                .Returns(true);

            mockDataStore
                .Setup(store => store.GetRepository<ViewTemplate>())
                .Returns(mockViewTemplateRepository.Object);

            var handler = new CreateNewViewOnPageTypeCreation(mockDataStore.Object);

            // Act
            await handler.Handle(pageTypeCreatedEvent, CancellationToken.None);

            // Assert
            mockViewTemplateRepository.Verify(repo => repo.Create(It.IsAny<ViewTemplate>()), Times.Never);
            mockDataStore.Verify(store => store.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}