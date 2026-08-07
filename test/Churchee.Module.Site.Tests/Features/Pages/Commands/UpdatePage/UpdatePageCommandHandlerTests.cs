using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.ResponseTypes;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Site.Entities;
using Churchee.Module.Site.Features.Pages.Commands.UpdatePage;
using Churchee.Module.Site.Specifications;
using Moq;
using System.Text.Json;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.UpdatePage
{
    public class UpdatePageCommandHandlerUpdateContentTests
    {
        private UpdatePageCommandHandler CreateHandler(
            Mock<IImageProcessor>? imageProcessor = null,
            Mock<IJobService>? jobService = null)
        {
            var storage = new Mock<Churchee.Common.Storage.IDataStore>();
            var ip = imageProcessor ?? new Mock<IImageProcessor>();
            var js = jobService ?? new Mock<IJobService>();
            var currentUser = new Mock<ICurrentUser>();

            return new UpdatePageCommandHandler(storage.Object, ip.Object, js.Object, currentUser.Object);
        }


        [Fact]
        public async Task Handle_UpdatesPageAndPublishesWhenRequested()
        {
            var storageMock = new Mock<IDataStore>();
            var pageRepoMock = new Mock<IRepository<Page>>();
            var imageProcessorMock = new Mock<IImageProcessor>();
            var jobServiceMock = new Mock<IJobService>();
            var currentUserMock = new Mock<ICurrentUser>();
            var page = new Page(System.Guid.NewGuid(), "title", "/url", "desc", System.Guid.NewGuid(), null, true);

            pageRepoMock.Setup(r => r.ApplySpecification(It.IsAny<PageWithContentAndPropertiesSpecification>())).Returns(new[] { page }.AsQueryable());

            storageMock.Setup(s => s.GetRepository<Page>()).Returns(pageRepoMock.Object);
            storageMock.Setup(s => s.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var handler = new UpdatePageCommandHandler(storageMock.Object, imageProcessorMock.Object, jobServiceMock.Object, currentUserMock.Object);

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

        [Fact]
        public async Task UpdateContent_NoContent_DoesNothing()
        {
            // Arrange
            var handler = CreateHandler();
            var page = new Page(Guid.NewGuid(), "t", "u", "m", Guid.NewGuid(), null, false);
            page.AddContent(Guid.NewGuid(), Guid.NewGuid(), "original", 1);
            var originalValue = page.PageContent.First().Value;
            var originalVersion = page.PageContent.First().Version;

            // Act
            await handler.UpdateContent(null, page, Guid.NewGuid(), CancellationToken.None);
            await handler.UpdateContent(new List<KeyValuePair<Guid, string>>(), page, Guid.NewGuid(), CancellationToken.None);

            // Assert
            Assert.Equal(originalValue, page.PageContent.First().Value);
            Assert.Equal(originalVersion, page.PageContent.First().Version);
        }

        [Fact]
        public async Task UpdateContent_NonImage_UpdatesValueAndIncrementsVersion()
        {
            // Arrange
            var handler = CreateHandler();
            var page = new Page(Guid.NewGuid(), "t", "u", "m", Guid.NewGuid(), null, false);

            var contentId = Guid.NewGuid();
            page.AddContent(contentId, Guid.NewGuid(), "old-value", 2);

            // set a non-image PageTypeContent
            var pct = new PageTypeContent(contentId, page.ApplicationTenantId, "Text", true, "Name", 0);
            page.PageContent.First().PageTypeContent = pct;

            var updates = new List<KeyValuePair<Guid, string>>
            {
                new KeyValuePair<Guid, string>(contentId, "new-value")
            };

            // Act
            await handler.UpdateContent(updates, page, Guid.NewGuid(), CancellationToken.None);

            // Assert
            var pc = page.PageContent.First();
            Assert.Equal("new-value", pc.Value);
            Assert.Equal(3, pc.Version); // incremented
        }

        [Fact]
        public async Task UpdateContent_ImageType_WithNoTempUrl_OnlyUpdatesAltTextAndSerializes()
        {
            // Arrange
            var handler = CreateHandler();
            var page = new Page(Guid.NewGuid(), "t", "u", "m", Guid.NewGuid(), null, false);

            var contentId = Guid.NewGuid();
            // existing image data (empty)
            page.AddContent(contentId, Guid.NewGuid(), string.Empty, 1);

            var pct = new PageTypeContent(contentId, page.ApplicationTenantId, "Image", true, "ImageName", 0);
            var pageContent = page.PageContent.First();
            pageContent.PageTypeContent = pct;

            var newImage = new ImageSimple { Url = "/img/original", TempUrl = string.Empty, AltText = "alt-text" };
            var updates = new List<KeyValuePair<Guid, string>>
            {
                new KeyValuePair<Guid, string>(contentId, JsonSerializer.Serialize(newImage))
            };

            // Act
            await handler.UpdateContent(updates, page, Guid.NewGuid(), CancellationToken.None);

            // Assert
            var pc = page.PageContent.First();
            var deserialized = JsonSerializer.Deserialize<ImageSimple>(pc.Value);
            Assert.NotNull(deserialized);
            Assert.Equal("/img/original", deserialized.Url);
            Assert.Equal(string.Empty, deserialized.TempUrl);
            Assert.Equal("alt-text", deserialized.AltText);
            Assert.Equal(2, pc.Version); // incremented
        }

        [Fact]
        public async Task UpdateContent_ImageType_WithTempUrl_CallsImageProcessorAndQueuesJob()
        {
            // Arrange
            var imageProcessorMock = new Mock<IImageProcessor>();
            var jobServiceMock = new Mock<IJobService>();

            var handler = CreateHandler(imageProcessorMock, jobServiceMock);
            var applicationTenantId = Guid.NewGuid();

            var page = new Page(applicationTenantId, "t", "u", "m", Guid.NewGuid(), null, false);

            var contentId = Guid.NewGuid();
            page.AddContent(contentId, Guid.NewGuid(), string.Empty, 5);

            var pct = new PageTypeContent(contentId, page.ApplicationTenantId, "Image", true, "ImageName", 0);
            var pageContent = page.PageContent.First();
            pageContent.PageTypeContent = pct;

            var newImage = new ImageSimple { Url = "file.png", TempUrl = "/temp/path.png", AltText = "alt" };
            var updates = new List<KeyValuePair<Guid, string>>
            {
                new KeyValuePair<Guid, string>(contentId, JsonSerializer.Serialize(newImage))
            };

            var returnedFinalUrl = "/img/pages/final.webp";
            imageProcessorMock
                .Setup(x => x.ConvertTempImageToFullImage(newImage.TempUrl, newImage.Url, It.IsAny<string>(), applicationTenantId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(returnedFinalUrl);

            // Act
            await handler.UpdateContent(updates, page, applicationTenantId, CancellationToken.None);

            // Assert
            imageProcessorMock.Verify(x => x.ConvertTempImageToFullImage(
                It.Is<string>(p => p == newImage.TempUrl),
                It.Is<string>(f => f == newImage.Url),
                It.IsAny<string>(),
                It.Is<Guid>(g => g == applicationTenantId),
                It.IsAny<CancellationToken>()),
                Times.Once);

            jobServiceMock.Verify(x => x.QueueJob<Churchee.ImageProcessing.Jobs.ImageCropsGenerator>(It.IsAny<System.Linq.Expressions.Expression<Func<Churchee.ImageProcessing.Jobs.ImageCropsGenerator, Task>>>()), Times.Once);

            var pc = page.PageContent.First();
            var deserialized = JsonSerializer.Deserialize<ImageSimple>(pc.Value);
            Assert.NotNull(deserialized);
            Assert.Equal(returnedFinalUrl.Replace(".webp", ""), deserialized.Url);
            Assert.Equal(string.Empty, deserialized.TempUrl);
            Assert.Equal("alt", deserialized.AltText);
            Assert.Equal(6, pc.Version); // incremented
        }
    }
}