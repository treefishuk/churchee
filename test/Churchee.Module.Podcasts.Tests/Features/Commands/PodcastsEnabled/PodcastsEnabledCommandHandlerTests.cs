using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Storage;
using Churchee.Module.Podcasts.Features.Commands;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tenancy.Entities;
using FluentAssertions;
using Moq;

namespace Churchee.Module.Podcasts.Tests.Features.Commands.PodcastsEnabled
{
    public class PodcastsEnabledCommandHandlerTests
    {
        private readonly Mock<IDataStore> _dataStoreMock;
        private readonly Mock<ICurrentUser> _currentUserMock;
        private readonly Mock<ISettingStore> _settingStoreMock;

        public PodcastsEnabledCommandHandlerTests()
        {
            _dataStoreMock = new Mock<IDataStore>();
            _currentUserMock = new Mock<ICurrentUser>();
            _settingStoreMock = new Mock<ISettingStore>();
        }

        [Fact]
        public async Task PodcastsEnabledCommandHandler_EnablesPodcasts()
        {
            //arrange
            var pageNameForPodcasts = "Podcasts";
            var command = new PodcastsEnabledCommand(pageNameForPodcasts);

            var mockEventRepository = new Mock<IRepository<PageType>>();
            var viewTemplateRepository = new Mock<IRepository<ViewTemplate>>();
            var applicationTenantRepository = new Mock<IRepository<ApplicationTenant>>();
            var pageRepository = new Mock<IRepository<Page>>();
            applicationTenantRepository.Setup(s => s.GetByIdAsync(It.IsAny<Guid>(), CancellationToken.None)).ReturnsAsync(new ApplicationTenant(Guid.NewGuid(), "Name", 1));

            _dataStoreMock.Setup(s => s.GetRepository<PageType>()).Returns(mockEventRepository.Object);
            _dataStoreMock.Setup(s => s.GetRepository<ViewTemplate>()).Returns(viewTemplateRepository.Object);
            _dataStoreMock.Setup(s => s.GetRepository<ApplicationTenant>()).Returns(applicationTenantRepository.Object);
            _dataStoreMock.Setup(s => s.GetRepository<Page>()).Returns(pageRepository.Object);


            var handler = new PodcastsEnabledCommandHandler(_dataStoreMock.Object, _currentUserMock.Object, _settingStoreMock.Object);

            //act
            var result = await handler.Handle(command, CancellationToken.None);

            //assert
            result.IsSuccess.Should().BeTrue();
        }
    }
}
