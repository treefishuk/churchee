using Ardalis.Specification;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.ChurchSuite.API;
using Churchee.Module.ChurchSuite.Jobs;
using Churchee.Module.Events.Entities;
using Churchee.Module.Site.Entities;
using Churchee.Module.Tokens.Entities;
using Churchee.Test.Helpers.Validation;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

namespace Churchee.Module.ChurchSuite.Tests.Jobs
{
    public class SyncChurchSuiteEventsJobTests
    {

        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly Mock<ISettingStore> _settingStore = new();
        private readonly Mock<IDataStore> _dataStore = new();
        private readonly Mock<IBlobStore> _blobStore = new();
        private readonly Mock<ILogger<SyncChurchSuiteEventsJob>> _logger = new();
        private readonly Mock<IJobService> _jobService = new();
        private readonly Mock<IImageProcessor> _imageProcessor = new();

        private readonly Mock<IRepository<Token>> tokenRepoMock = new();
        private readonly Mock<IRepository<Event>> eventRepoMock = new();
        private readonly Mock<IRepository<EventDate>> eventDateRepoMock = new();
        private readonly Mock<IRepository<PageType>> pageTypeRepoMock = new();
        private readonly Mock<IRepository<Page>> pageRepoMock = new();

        private readonly Guid tenantId;

        public SyncChurchSuiteEventsJobTests()
        {

            tenantId = Guid.NewGuid();

            _dataStore.Setup(x => x.GetRepository<Token>()).Returns(tokenRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Event>()).Returns(eventRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<PageType>()).Returns(pageTypeRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<Page>()).Returns(pageRepoMock.Object);
            _dataStore.Setup(x => x.GetRepository<EventDate>()).Returns(eventDateRepoMock.Object);

            tokenRepoMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Token>>(), It.IsAny<Expression<Func<Token, string>>>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync("access-token");

            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("page-id");

            _logger.Setup(s => s.IsEnabled(LogLevel.Error)).Returns(true);

        }

        [Fact]
        public async Task GetFeedResult_With_Test_Api_Returns_Data()
        {
            // Arrange
            _settingStore.Setup(x => x.GetSettingValue(It.IsAny<Guid>(), tenantId)).ReturnsAsync("https://demo.churchsuite.com/embed/calendar/json");

            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(new HttpClient());

            var cut = new SyncChurchSuiteEventsJob(_httpClientFactory.Object, _settingStore.Object, _dataStore.Object, _blobStore.Object, _jobService.Object, _imageProcessor.Object, _logger.Object);

            var result = await cut.GetFeedResult(tenantId);

            result.Count().Should().BeGreaterThan(0);


        }
    }
}
