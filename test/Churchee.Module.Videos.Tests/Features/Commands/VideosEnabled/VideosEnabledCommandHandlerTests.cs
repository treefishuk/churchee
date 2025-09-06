using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using Churchee.Module.Videos.Helpers;
using Moq;

namespace Churchee.Module.Videos.Features.Commands
{
    public class VideosEnabledCommandHandlerTests
    {
        [Fact]
        public async Task Handle_CreatesPagesAndTemplates_WhenSettingsAreMissing()
        {
            // Arrange
            var dataStoreMock = new Mock<IDataStore>(MockBehavior.Strict);
            var currentUserMock = new Mock<ICurrentUser>(MockBehavior.Strict);
            var settingStoreMock = new Mock<ISettingStore>(MockBehavior.Strict);
            var applicationTenantId = Guid.NewGuid();
            string pageName = "videos";
            var command = new VideosEnabledCommand(pageName);
            var tenant = new ApplicationTenant(applicationTenantId, "TestTenant", 123);

            currentUserMock.Setup(x => x.GetApplicationTenantId()).ReturnsAsync(applicationTenantId);
            settingStoreMock.Setup(x => x.GetSettingValue(Settings.VideosNameId, applicationTenantId)).ReturnsAsync((string?)null);
            settingStoreMock.Setup(x => x.AddSetting(Settings.VideosNameId, applicationTenantId, It.IsAny<string>(), pageName)).Returns(Task.CompletedTask);

            var appTenantRepo = new Mock<IRepository<ApplicationTenant>>();
            appTenantRepo.Setup(r => r.GetByIdAsync(applicationTenantId, It.IsAny<CancellationToken>())).ReturnsAsync(tenant);
            dataStoreMock.Setup(x => x.GetRepository<ApplicationTenant>()).Returns(appTenantRepo.Object);

            var pageRepo = new Mock<IRepository<Page>>();
            pageRepo.Setup(r => r.GetQueryable()).Returns(new System.Collections.Generic.List<Page>().AsQueryable());
            pageRepo.Setup(r => r.Create(It.IsAny<Page>()));
            dataStoreMock.Setup(x => x.GetRepository<Page>()).Returns(pageRepo.Object);

            var viewTemplateRepo = new Mock<IRepository<ViewTemplate>>();
            viewTemplateRepo.Setup(r => r.GetQueryable()).Returns(new System.Collections.Generic.List<ViewTemplate>().AsQueryable());
            viewTemplateRepo.Setup(r => r.Create(It.IsAny<ViewTemplate>()));
            dataStoreMock.Setup(x => x.GetRepository<ViewTemplate>()).Returns(viewTemplateRepo.Object);

            var pageTypeRepo = new Mock<IRepository<PageType>>();
            pageTypeRepo.Setup(r => r.AnyWithFiltersDisabled(It.IsAny<System.Linq.Expressions.Expression<Func<PageType, bool>>>())).Returns(false);
            pageTypeRepo.Setup(r => r.Create(It.IsAny<PageType>()));
            dataStoreMock.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepo.Object);

            dataStoreMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new VideosEnabledCommandHandler(dataStoreMock.Object, currentUserMock.Object, settingStoreMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<CommandResponse>(result);
        }
    }
}
