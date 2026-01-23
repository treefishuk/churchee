using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands.CreatePage;
using Churchee.Module.Site.Specifications;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.CreatePage
{
    public class CreatePageCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesPageAndSavesChanges()
        {
            // Arrange
            var storageMock = new Mock<IDataStore>();
            var repoMock = new Mock<IRepository<Page>>();
            var currentUserMock = new Mock<ICurrentUser>();

            var applicationTenantId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var pageTypeId = Guid.NewGuid();

            currentUserMock.Setup(x => x.GetApplicationTenantId())
                .ReturnsAsync(applicationTenantId);

            // Mock FirstOrDefaultAsync to return a parent slug
            repoMock.Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<PageFromParentIdSpecification>(),
                    It.IsAny<Expression<Func<Page, string>>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync("parent-slug");

            storageMock.Setup(x => x.GetRepository<Page>())
                .Returns(repoMock.Object);

            var handler = new CreatePageCommandHandler(storageMock.Object, currentUserMock.Object);

            var command = new CreatePageCommand("Test Title", "Test Description", pageTypeId.ToString(), parentId.ToString());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            repoMock.Verify(x => x.Create(It.IsAny<Page>()), Times.Once);
            storageMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
        }
    }
}
