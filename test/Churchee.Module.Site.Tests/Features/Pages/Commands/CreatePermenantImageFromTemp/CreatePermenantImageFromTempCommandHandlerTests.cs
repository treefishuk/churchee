using Churchee.Common.Abstractions.Auth;
using Churchee.Common.Abstractions.Queue;
using Churchee.Common.Abstractions.Utilities;
using Churchee.Common.Storage;
using Churchee.Common.ValueTypes;
using Churchee.Module.Site.Features.Pages.Commands.CreatePermenantImageFromTemp;
using Churchee.Test.Helpers.Validation;
using Moq;

namespace Churchee.Module.Site.Tests.Features.Pages.Commands.CreatePage
{
    public class CreatePermenantImageFromTempTests
    {
        [Fact]
        public async Task Handle_Deletes_Temp_File()
        {
            // Arrange
            var tenantId = Guid.NewGuid();
            string tempFilePath = Path.Combine(Path.GetTempPath(), "text.png");
            using var tempfile = File.Create(tempFilePath);
            tempfile.Close();

            var imageProcessorMock = new Mock<IImageProcessor>();
            var blobStoreMock = new Mock<IBlobStore>();
            var currentUserMock = new Mock<ICurrentUser>();
            var jobServiceMock = new Mock<IJobService>();

            using var memoryStream = new MemoryStream();

            currentUserMock.Setup(s => s.GetApplicationTenantId()).ReturnsAsync(tenantId);
            imageProcessorMock.Setup(s => s.ConvertToWebP(It.IsAny<Stream>(), It.IsAny<CancellationToken>())).ReturnsAsync(memoryStream);
            blobStoreMock.Setup(s => s.SaveAsync(tenantId, "final/test.webp", It.IsAny<Stream>(), false, It.IsAny<CancellationToken>())).ReturnsAsync("final/test.webp");

            var chunk = new ChunkedUploadRequest() { FileName = "test.txt", TempFilePath = tempFilePath, FinalFilePath = "final/test" };

            var command = new CreatePermenantImageFromTempCommand(chunk);

            var handler = new CreatePermenantImageFromTempCommandHandler(imageProcessorMock.Object, blobStoreMock.Object, currentUserMock.Object, jobServiceMock.Object);

            // Act
            string result = await handler.Handle(command, CancellationToken.None);

            // Assert
            File.Exists(tempFilePath).Should().BeFalse();
        }
    }
}
