using Ardalis.Specification;
using Churchee.Common.Abstractions.Storage;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Module.Events.Entities;
using Churchee.Module.Events.Jobs;
using Hangfire;
using Moq;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Churchee.Module.Events.Tests.Jobs
{
    public class EventImageProcessingJobTests
    {
        [Fact]
        public async Task ProcessAsync_WhenImageDataIsEmpty_DoesNothing()
        {
            var mockDataStore = new Mock<IDataStore>();
            var mockBlobStore = new Mock<IBlobStore>();
            var mockImageProcessor = new Mock<IImageProcessor>();
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();

            var job = new EventImageProcessingJob(
                mockDataStore.Object,
                mockBlobStore.Object,
                mockImageProcessor.Object,
                mockBackgroundJobClient.Object);

            await job.ProcessAsync(Guid.NewGuid(), Guid.NewGuid(), "event-image.jpg", Array.Empty<byte>(), CancellationToken.None);

            mockImageProcessor.Verify(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()), Times.Never);
            mockBlobStore.Verify(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Never);
            mockDataStore.Verify(x => x.GetRepository<Event>(), Times.Never);
        }

        [Fact]
        public async Task ProcessAsync_WhenEventExists_SetsImageUrlAndPublishesEvent()
        {
            var applicationTenantId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var mockDataStore = new Mock<IDataStore>();
            var mockBlobStore = new Mock<IBlobStore>();
            var mockImageProcessor = new Mock<IImageProcessor>();
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            var mockRepository = new Mock<IRepository<Event>>();

            var eventEntity = new Event.Builder()
                .SetApplicationTenantId(applicationTenantId)
                .SetTitle("Event Title")
                .SetDescription("Event description")
                .SetImageUrl(string.Empty)
                .SetPublished(false)
                .Build();

            mockRepository
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Event>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(eventEntity);

            mockDataStore.Setup(x => x.GetRepository<Event>()).Returns(mockRepository.Object);
            mockDataStore.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            mockImageProcessor
                .Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream _, CancellationToken _) => new MemoryStream(CreateWebpBytes()));

            mockBlobStore
                .Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid _, string path, Stream _, bool _, CancellationToken _) => path);

            mockBlobStore
                .Setup(x => x.GetReadStreamAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid _, string _, CancellationToken _) => CreateImageStream());

            var job = new EventImageProcessingJob(
                mockDataStore.Object,
                mockBlobStore.Object,
                mockImageProcessor.Object,
                mockBackgroundJobClient.Object);

            await job.ProcessAsync(applicationTenantId, eventId, "Event Image.jpg", CreateImageBytes(), CancellationToken.None);

            Assert.Equal("/img/events/eventImage.webp", eventEntity.ImageUrl);
            Assert.True(eventEntity.Published);
            mockDataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockBlobStore.Verify(x => x.SaveAsync(
                    applicationTenantId,
                    It.Is<string>(path => path == "/img/events/eventImage.webp"),
                    It.IsAny<Stream>(),
                    false,
                    It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task ProcessAsync_WhenEventIsNotFound_DoesNotSaveChanges()
        {
            var applicationTenantId = Guid.NewGuid();
            var eventId = Guid.NewGuid();

            var mockDataStore = new Mock<IDataStore>();
            var mockBlobStore = new Mock<IBlobStore>();
            var mockImageProcessor = new Mock<IImageProcessor>();
            var mockBackgroundJobClient = new Mock<IBackgroundJobClient>();
            var mockRepository = new Mock<IRepository<Event>>();

            mockRepository
                .Setup(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Event>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Event)null!);

            mockDataStore.Setup(x => x.GetRepository<Event>()).Returns(mockRepository.Object);

            mockImageProcessor
                .Setup(x => x.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Stream _, CancellationToken _) => new MemoryStream(CreateWebpBytes()));

            mockBlobStore
                .Setup(x => x.SaveAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<Stream>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid _, string path, Stream _, bool _, CancellationToken _) => path);

            mockBlobStore
                .Setup(x => x.GetReadStreamAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Guid _, string _, CancellationToken _) => CreateImageStream());

            var job = new EventImageProcessingJob(
                mockDataStore.Object,
                mockBlobStore.Object,
                mockImageProcessor.Object,
                mockBackgroundJobClient.Object);

            await job.ProcessAsync(applicationTenantId, eventId, "event-image.jpg", CreateImageBytes(), CancellationToken.None);

            mockRepository.Verify(x => x.FirstOrDefaultAsync(It.IsAny<ISpecification<Event>>(), It.IsAny<CancellationToken>()), Times.Once);
            mockDataStore.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private static byte[] CreateImageBytes()
        {
            using var image = new Image<Rgba32>(200, 120);
            using var stream = new MemoryStream();
            image.SaveAsJpeg(stream);
            return stream.ToArray();
        }

        private static byte[] CreateWebpBytes()
        {
            using var image = new Image<Rgba32>(200, 120);
            using var stream = new MemoryStream();
            image.SaveAsWebp(stream);
            return stream.ToArray();
        }

        private static Stream CreateImageStream()
        {
            return new MemoryStream(CreateImageBytes());
        }
    }
}
